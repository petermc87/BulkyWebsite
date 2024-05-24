using System.ComponentModel.DataAnnotations;

namespace BulkNess12.Models
{
    public class Category
    {
        // Columns in the table.
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
