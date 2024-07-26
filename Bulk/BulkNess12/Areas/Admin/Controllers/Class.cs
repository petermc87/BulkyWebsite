using Bulky.Models;
using BulkyWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BulkNess12.Areas.Admin.Controllers
{
    public class ProductController : Product
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //--- READ ---//
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();

            //TODO: Add view here
            // return View(objProductList)

        }

        //TODO:
        
        //--- CREATE ---//
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Create(Product obj)
            if (object.)
    }
}



