using Bulky.Utility;
using BulkyWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkNess12.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        // Retrieving shopping cart - dependency injection
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // For getting the user logged in.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // If the claim returned a logged in user
            // NOTE: This is for seeing the shopping cart count in the nav.
            if (claim != null)
            {
                // If there is no existing session
                if(HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    // See the session
                    HttpContext.Session.SetInt32(SD.SessionCart,
                    // Checking if the shopping cart ApplicationUser is equal to the claim user
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());

                }
               // Returnin the session cart # to the view.
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            } 
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
