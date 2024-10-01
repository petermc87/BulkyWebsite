using Bulky.Models;
using BulkyWeb.DataAccess.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository
{
    // NOTE: If you hit CTRL + . over the ICategoryRepository section it will create
    // the interface including the Add, Remove, etc which are already defined in
    // the Repository file.
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        // Declaring the db context here again
        private ApplicationDbContext _db;
        // Inject the db context dependency to the CategoryRepository.
        // This then is passed to the base class i.e. Repository<Category>
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
  

        public void Update(OrderDetail obj)
        {
            _db.OrderDetail.Update(obj);
        }
    }
}
