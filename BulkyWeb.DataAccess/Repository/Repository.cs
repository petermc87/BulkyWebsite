using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BulkyWeb.DataAccess.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.DataAccess.Repository
{
    // T is the Generic class
    public class Repository<T> : IRepository<T> where T : class
    {
        // Private readonly field to store the application's database context.
        private readonly ApplicationDbContext _db;

        // Internal DbSet variable to represent a collection of entities of type T.
        internal DbSet<T> dbSet; 

        // Constructor for the Repository class, which takes an ApplicationDbContext as a parameter.
        public Repository(ApplicationDbContext db)
        {
            // Initialize the private _db field with the provided database context.
            _db = db;

            // Assign the DbSet for the generic type T from the database context to the dbSet variable.
            this.dbSet = _db.Set<T>();

            // Including the categoy value when fetching the products. You can use as many includes as you need.
            _db.Products.Include(u => u.Category).Include(u => u.CategoryId);


        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query = dbSet;
            if (tracked)
            {
                query = dbSet;

            } else
            {

               query = dbSet.AsNoTracking();

            }
            query = query.Where(filter); // <-- Filtering one record from the the dbset
                                         // If there are includes, include them
            if (!string.IsNullOrEmpty(includeProperties))
            {
                // Splitting the string above that is passed in.
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        // -- Below is the comma separate string that gets passed in to the get all function.

        //Category,CoverType
        public IEnumerable<T> GetAll(string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            // If there are includes, include them
            if (!string.IsNullOrEmpty(includeProperties))
            {
                // Splitting the string above that is passed in.
                foreach(var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            
            return query.ToList(); // <-- Putting all the records into a list

        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
