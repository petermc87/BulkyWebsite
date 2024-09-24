using Bulky.Models;
using BulkyWeb.DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Bulky.Models.ViewModels;
// It was not picking up Authorize attribute from the package so I created
// a custom variable. There is a conflict somewhere!!
using AuthAttribute = Microsoft.AspNetCore.Authorization;
using Bulky.Utility;

namespace BulkNess12.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[AuthAttribute.Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        // --> FOR ADDING IMAGE <-- //
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        //public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        //--- READ ---//
        public IActionResult Index()
        {
            // Adding the Category property to the Product by using the includeProperties method.
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
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
        // --> CREATE & UPDATE <--//
        // Add in the form file, which is related to the image being uploaded in the form.
        public IActionResult Create(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                //--> COMMENTED OUT TO CHECK THE PREVIOUS CODE FUNCTION < -- //
                //wwwroot folder path
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    // Create a random file name in lieu of the current name + add preserve the file extension.
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    // Combining the root path and the relative folder path
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    // If there an image URL currently in there
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        // Need to trim off the backslash at the start of the URL)
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        // Delete the old image
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                    }




                    // Save file to the path
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    // Image URL, in relation to the ProductVM (downstream Product model), is referenced to the 
                    // stored image.
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                // If its a new product, the id will be 0, so Add new
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                // Otherwise, update
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product" + (@productVM.Product.Id == 0 ? "created" : "updated") + "successfully";
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

        // Using datatables.net to retrieve data from an API
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            // Adding the Category property to the Product by using the includeProperties method.
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            // Retrieving the object to be deleted.
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {

                return Json(new { success = true, message = "Error while deleting" });
            }


            // Deleting the old image
            var oldImagePath =
                Path.Combine(_webHostEnvironment.WebRootPath,
                productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }

        #endregion
    }
}