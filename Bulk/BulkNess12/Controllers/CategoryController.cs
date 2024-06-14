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

        //--- READ ---//
        public IActionResult Index()
        {
            // Retrieve all the categories as a list and store it in objCategoryList
            List<Category> objCategoryList = _db.Categories.ToList(); // <-- this is typed
            // Passing the list of categories into the View.
            return View(objCategoryList);
        }

        //--- CREATE ---//
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost] // <-- Http request for form posted in the Create.cshtml
        public IActionResult Create(Category obj)
        {
            if(obj.DisplayOrder.ToString() == obj.Name)
            {
                // --- CUSTOM VALIDATION (SERVER SIDE) ---//
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

        //--- UPDATE ---//

        // -- Retrieve the cat.
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Finding the cat by id
            Category? foundCategory = _db.Categories.Find(id);

            /// --- TESTING OTHER OPTIONS -- ///
            //// Isolating the id property of the Category object and matching it with the id passed in.
            //Category? foundCategory1 = _db.Categories.FirstOrDefault(u => u.Id == id);
            //// Isolating the id property and matching it with the id
            //Category? foundCategory2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            /// ------------------------------------------------------------------------------------- ///
            
            if (foundCategory == null) //<-- Error message if not found
            {
                return NotFound();
            }
            // Returning the cat if it was found to the view.
            return View(foundCategory);
        }

        // --> Update Cat in Db.
        [HttpPost]
        public IActionResult Edit(Category Obj)
        {
            if (ModelState.IsValid) // <-- Client side validate
            {
                _db.Categories.Add(Obj); // <-- Add the cat obj.
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
