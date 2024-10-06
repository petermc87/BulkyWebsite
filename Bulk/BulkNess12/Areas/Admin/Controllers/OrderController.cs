using BulkyWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Bulky.Models;

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

		// Using datatables.net to retrieve data from an API
		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			// Retrieving the orderheader list.
			List<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			return Json(new { data = objOrderHeaders});
		}


		#endregion

	}
}
