using BulktWebrazor_temp.Models;
using Microsoft.EntityFrameworkCore;

namespace BulktWebrazor_temp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Thriller", DisplayOrder = 4 },
                new Category { Id = 3, Name = "Horror", DisplayOrder = 3 }
                );
        }
    }
}
