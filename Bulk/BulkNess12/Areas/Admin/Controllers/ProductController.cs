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

        //--- UPSERT ---//
        public IActionResult Upsert(int? id) 
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
            if (id == null || id == 0)
            {
                // Create product
                return View(productVM);
            }
            else
            {
                // Update product.
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
         
        }
        [HttpPost]
        // Add in the form file, which is related to the image being uploaded in the form.
        public IActionResult Create(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                // Populate cat list dropdown (genre) in order to avoid the exception error.
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
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



