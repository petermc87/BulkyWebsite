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

        // Default entity framework builder for seeding
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Category>().HasData(
            //    // Data that will be seeded to the database
            //    new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
            //    new Category { Id = 2, Name = "Thriller", DisplayOrder = 2 },
            //    new Category { Id = 3, Name = "Horror", DisplayOrder = 3 }
            //    );

            // Additional context
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
