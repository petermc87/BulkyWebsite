using BulktWebrazor_temp.Data;
using BulktWebrazor_temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulktWebrazor_temp.Pages.Categories
{
    // Constructor
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        // Declare categoryList variable
        public List<Category> CategoryList { get; set; } //--> "This property is a publicly accessible list of Category objects and is named CategoryList."
        // Dependency Injection
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }
       
        public void OnGet() // NOTE: Void is used here because there is no return value.
        {
            CategoryList = _db.Categories.ToList();
        }

    }

}
