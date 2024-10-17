using BulkyWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Bulky.Models;
using Bulky.Utility;
using Bulky.Models.ViewModels;
// It was not picking up Authorize attribute from the package so I created
// a custom variable. There is a conflict somewhere!!
using AuthAttribute = Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace BulkNess12.Areas.Admin.Controllers
{
	[Area("Admin")]

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
