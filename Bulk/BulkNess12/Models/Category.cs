using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkNess12.Models
{
    public class Category
    {
        // Columns in the table.
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Category Name")] //<-- This is the text that is passed into the input on the front end.
        public string Name { get; set; }
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
