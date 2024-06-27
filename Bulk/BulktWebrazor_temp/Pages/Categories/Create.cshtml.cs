using BulktWebrazor_temp.Data;
using BulktWebrazor_temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulktWebrazor_temp.Pages.Categories
{
    [BindProperties]  //<-- This is to make the properties below available in the HTTP methods below
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category NewCategory { get; set; }
        
        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            _db.Categories.Add(NewCategory);
            _db.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
