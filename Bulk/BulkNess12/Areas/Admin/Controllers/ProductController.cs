using Bulky.Models;
using BulkyWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BulkNess12.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //public object ModelState { get; private set; }

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //--- READ ---//
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();

            //TODO: Add view here
            return View(objProductList);

        }

        //TODO:

//        //--- CREATE ---//
//        public IActionResult Create()
//        {
//            return View();
//        }

//        [HttpPost]

//        public IActionResult Create(Product obj)
//        {
//            // Checking the model state is valid
//            if (ModelState.IsValid)
//            {
//                _unitOfWork.Product.Add(obj);
//                _unitOfWork.Save();
//                TempData["success"] = "New Product has been created";
//                return RedirectToAction("Index");
//            }

//            return View();
//}
    }
}



