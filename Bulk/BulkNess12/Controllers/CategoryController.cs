using BulkNess12.Data;
using BulkNess12.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace BulkNess12.Controllers
{
    public class CategoryController : Controller
    {

        private readonly ApplicationDbContext _db;
        // Implementation of db context
        public CategoryController(ApplicationDbContext db)
        // Assign the db context to the local variable
        {
            _db = db;
        }
        public IActionResult Index()
        {
            // Retrieve all the categories as a list and store it in objCategoryList
            List<Category> objCategoryList = _db.Categories.ToList(); // <-- this is typed
            // Passing the list of categories into the View.
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost] // <-- Http request for form posted in the Create.cshtml
        public IActionResult Create(Category obj)
        {
            if(obj.DisplayOrder.ToString() == obj.Name)
            {

                ModelState.AddModelError("name", "The Category Name cannot exactly match the Display Order"); //<-- Adding an error message to the "name" HTML element in the front end when there is a match.
            }
            // --- NOTE:this is not required for the production app -- //
            //if(obj.Name != null && obj.Name.ToLower() == "test")
            //{
            //    ModelState.AddModelError("", "Test is not a valid name"); //<-- We are not binding this to a HTML element, so it will show up in the list at the top of ths container.
            //}
            if(ModelState.IsValid) // If the validation from the model is correct, then persist to db.
            {
                _db.Categories.Add(obj); //<-- Pointing to the obj to be saved to the db
                _db.SaveChanges();//<-- Saving to db.
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
