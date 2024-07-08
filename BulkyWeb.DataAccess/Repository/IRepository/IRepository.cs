using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository.IRepository
{
    // Generic Repository Pattern
    // NOTE: Update is that used in Generic because of how nuanced it is.
    internal interface IRepository<T> where T : class 
    {
        // T - Category
        IEnumerable<T> GetAll();
        // A representation of the link operation shown in the MVC controller: Category? foundCategory1 = _db.Categories.FirstOrDefault(u => u.Id == id);
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity); // <-- Removes all objects
        

    }
}
