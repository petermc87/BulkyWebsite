using BulkNess12.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkNess12.Data
{
    // A class file that implements the DbContext Entity Framework package
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) // --> base(options) will pass the configuration to the DbContext class
        {
            
        }
        // Command for creating Category table.
        public DbSet<Category> Categories { get; set; }
    }
}
