using BulkNess12.Data;
using BulkNess12.Models;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }
    }
}
