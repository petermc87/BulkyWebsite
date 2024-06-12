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
