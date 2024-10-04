using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BulkyWeb.DataAccess.Repository;
using Bulky.Models.ViewModels;
using System.Security.Claims;
using Bulky.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Bulky.Utility;
using Stripe.Checkout;
namespace BulkNess12.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        // The shoppingcart vm that is populated with the values from the ApplicationUser is bound to this ShoppinvgCartVM to be
        // used in the Post IActionMethod.
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            // Grabbing the userid.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };
            // Indexing the cart in the VM list and adding the cart total based on the pricing parameters
            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
               cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            // Grabbing the userid.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            // Retrieving the current users info object.
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            // Adding pairing up the key:values in OrderHeader.
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            // Indexing the cart in the VM list and adding the cart total based on the pricing parameters
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

            }

            return View(ShoppingCartVM);

        }

        [HttpPost]
        // Here, the Summary get is separated from the Post action.
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            // Get the user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUser.Id == userId, includeProperties: "Product");


            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            // NOTE: No mapping like in the previous action method because the user details will be auto populated.
            // ALSO NOTE: Create a new applicationUser object because the original object is a navigation object.
            // You cannot use a navigation with a new object being submitted.
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);



			// Indexing the cart in the VM list and adding the cart total based on the pricing parameters
			foreach (var cart in ShoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

			}

            if(applicationUser.CompanyId.GetValueOrDefault() == 0){
                // it is regular customer account and payment needs to be captured
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            } else {
                // it is a company user
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }




            // Create order header
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();


            // Save order details to db for each item in the shopping cart.
            foreach (var cart in ShoppingCartVM.ShoppingCartList) {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count

                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            // CUSTOMER PAYMENT - STRIPE
            if(applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // This will be regular customer because the company Id is 0
                // Stripe logic is added here.
                var domain = "https://localhost:7165/";
                var options = new SessionCreateOptions
                {
                    // Confirmation page upon submission
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach(var items in ShoppingCartVM.ShoppingCartList)
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
                _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                // Return payment to stripe
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation), new { id=ShoppingCartVM.OrderHeader.Id});
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }


        // Cart update action methods.
        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            // Redirecting to the shopping cart index, not the Home index!!
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
           // If item is 1, the we remove from cart
           if(cartFromDb.Count <= 1) {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            } else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }


            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }



        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            // Returning the price based on quantity (the more they order the lower the unit price)
            if(shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            } else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                } else
                {
                    return shoppingCart.Product.Price100;
                }
            }

        }
    }
}
