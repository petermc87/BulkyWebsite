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
    // NOTE: If you hit CTRL + . over the IOrderHeaderRepository section it will create
    // the interface including the Add, Remove, etc which are already defined in
    // the Repository file.
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        // Declaring the db context here again
        private ApplicationDbContext _db;
        // Inject the db context dependency to the OrderHeaderRepository.
        // This then is passed to the base class i.e. Repository<OrderHeader>
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
  

        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if(orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
		}

		public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if(!string.IsNullOrEmpty(sessionId)){
                orderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
		}


	}
}
