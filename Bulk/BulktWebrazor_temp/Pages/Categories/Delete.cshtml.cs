using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BulktWebrazor_temp.Data;
using BulktWebrazor_temp.Models;
using Microsoft.EntityFrameworkCore;


namespace BulktWebrazor_temp.Pages.Categories

{
    [BindProperties]
    public class DeleteModel : PageModel

    {
        private readonly ApplicationDbContext _db;
        public Category Category { get; set; }
        public DeleteModel(ApplicationDbContext db)

        {
            _db = db;
        }

        public void OnGet(int? id)
        {
            if (id != null || id != 0)
            {
                Category = _db.Categories.Find(id);
            }
        }

        public IActionResult OnPost()

        {
            // Find the category object and by the id 
            // Finding cat obj to be deleted 
            Category? obj = _db.Categories.Find(Category.Id);
            // Error Case  
            if (obj == null)
            {
                return NotFound();
            }

            //Category? obj  
            _db.Categories.Remove(obj);
            TempData["success"] = "Category Delete Successfully";
            _db.SaveChanges();
            return RedirectToPage("Index");

        }

    }

}