using BulkyWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Bulky.Models;
using Bulky.Utility;
using Bulky.Models.ViewModels;

namespace BulkNess12.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;


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
			OrderVM orderVM = new ()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};
            return View(orderVM);
        }

        // Using datatables.net to retrieve data from an API
        #region API CALLS

        [HttpGet]
		public IActionResult GetAll(string status)
		{
			// Retrieving the orderheader list.
			IEnumerable<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			
			switch(status)
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
