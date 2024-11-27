using BulkyWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Bulky.Models;
using Bulky.Utility;
using Bulky.Models.ViewModels;
// It was not picking up Authorize attribute from the package so I created
// a custom variable. There is a conflict somewhere!!
using AuthAttribute = Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Stripe;
using Stripe.Checkout;


namespace BulkNess12.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
        // Binds form data to the OrderVM property automatically during POST requests, 
        // eliminating the need to pass OrderVM as a parameter in action methods.
        private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }

		// Generating a class and passing in a variable called unitOfwork with a type IUnitOfWork
		public OrderController(IUnitOfWork unitOfWork)
		{
			// Assignment.
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Details(int orderId)
        {
			OrderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};
            return View(OrderVM);
        }


		[HttpPost]
        [AuthAttribute.Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

			//These will only be updated if the field is empty.
			if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
			{
				orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
			}
			if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
			{
				orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			}

			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();

			TempData["Success"] = "Order Details Updated Successfully.";

			return RedirectToAction(nameof(Details), new {orderId=orderHeaderFromDb.Id});
        }


		[HttpPost]
		[AuthAttribute.Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
			_unitOfWork.Save();
			TempData["Success"] = "Order successfully updated";
			return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
		}


        [HttpPost]
        [AuthAttribute.Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        public IActionResult ShipOrder()
        {
			// Header needs to be retrieved so all the data can be updated.
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			orderHeader.OrderStatus = SD.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;
			if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
			}

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order shipped successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }


        [HttpPost]
        [AuthAttribute.Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            // Header needs to be retrieved so all the data can be updated.
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

			// Payment complete, refund required
			if(orderHeader.PaymentStatus == SD.PaymentStatusApproved)
			{
				// Stripe refund methods
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId,
				};

				// Creating a refund service (stripe)
				var service = new RefundService();
				Refund refund = service.Create(options);


				// Chaning the status of the order in the orderheader.
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
			} else
			{
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Order cancelled successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

		[ActionName("Details")]
		[HttpPost]
		public IActionResult Detail_PAY_NOW()
		{

			OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

            // This will be regular customer because the company Id is 0
            // Stripe logic is added here.
            var domain = "https://localhost:7165/";
            var options = new SessionCreateOptions
            {
                // Confirmation page upon submission
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var items in OrderVM.OrderDetail)
            {
                // Line items for the current stripe session (this is based on the 
                // items in the shopping cart)
                // Payment template from: https://docs.stripe.com/api/checkout/sessions/create#create_checkout_session-tax_id_collection
                var sessionLineItems = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(items.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = items.Product.Title
                        }
                    },
                    Quantity = items.Count
                };
                options.LineItems.Add(sessionLineItems);
            }

            var service = new SessionService();
            // Create session based on the options from the foreach above
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            // Return payment to stripe
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
		}
        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            // Retrieve order header
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);

            // Basically if the status IS delayed, then its can be processed further
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                // This is a company order.
                // Create a session
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                // Checking the payment status in relation to the Stripe "Payment Status" in Create a Session section.
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }



            return View(orderHeaderId);
        }
        // Using datatables.net to retrieve data from an API
        #region API CALLS

        [HttpGet]
		public IActionResult GetAll(string status)
		{
            // Creating a list called objOrderHeaders of type OrderHeaders.
            IEnumerable<OrderHeader> objOrderHeaders;
			

			if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)){
				// Get all the order headers that are either admin or employee
				objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			}
			else
			{

                // Grabbing the userid.
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");

            }


            switch (status)
			{
				case "pending":
					objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
					break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
				default:
					break;

            }
			return Json(new { data = objOrderHeaders});
		}


		#endregion

	}
}
