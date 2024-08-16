using Bulky.Models;
using BulkyWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Models.ViewModels;

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

            return View(objProductList);

        }

        //--- CREATE ---//
        public IActionResult Create() 
        {
            ProductVM productVM = new()
            {
                //  Generating a new list item to be added to the CategoryList
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            return View(productVM);
        }
        [HttpPost]

        public IActionResult Create(ProductVM obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(obj.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }


            // Picking the Name and Id columns of the Category and adding it the product list for data view
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            // Viewbag is used for passing data from Controller to View
            ViewBag.CategoryList = CategoryList;

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

            if(foundProduct == null)
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

        public IActionResult Delete(int? id)
        {
            if(id == 0 || id == null)
            {
                return NotFound();
            }

            Product? returnedProduct = _unitOfWork.Product.Get(u => u.Id == id);

            if (returnedProduct == null)
            {
                return NotFound();
            }
            return View(returnedProduct); 
        }
        [HttpPost, ActionName("Delete")]

        public IActionResult DeletePOST(int? id)
        {
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);

            if(obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Successfully deleted product";
            return RedirectToAction("index");
        }
    }
}



