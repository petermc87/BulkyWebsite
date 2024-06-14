using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkNess12.Models
{
    public class Category
    {
        // Columns in the table.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)] // <-- validation state that checks if its less than or equal to 30 chars.
        [DisplayName("Category Name")] //<-- This is the text that is passed into the input on the front end.
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1,100,ErrorMessage ="Display Order must be between 1 and 100")]
        public int DisplayOrder { get; set; }
    }
}
