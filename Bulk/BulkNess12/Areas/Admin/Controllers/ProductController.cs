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

        //--- CREATE ---//
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Create(Product obj)
        {
            // Maybe create a custom validation here?
            // Checking the model state is valid
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "New Product has been created";
                return RedirectToAction("Index");
            }

            return View();
        }

        //--- UPDATE ---//
        public IActionResult Edit(int? id)
        {
            if(id == 0 || id == null)
            {
                return NotFound();
            }

            // Finding the product by the id.
            Product? foundProduct = _unitOfWork.Product.Get(u => u.Id == id);

            if(foundProduct != null)
            {
                return NotFound();
            }

            //Passing the object to the view if found
            return View(foundProduct);
        }

        // Updating the db

        [HttpPost]
        public IActionResult Edit(Product Obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(Obj);
                _unitOfWork.Save();
                TempData["success"] = "Product Updated";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}



