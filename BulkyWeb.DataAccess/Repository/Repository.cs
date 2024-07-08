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
    internal class Repository<T> : IRepository<T> where T : class
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
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter); // <-- Filtering one record from the the dbset
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet; 
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
