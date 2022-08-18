using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_DataAccess.Services
{
    public class OrderService : Repository<Order>, IOrder
    {
        private readonly ApplicationDbContext _db;

        public OrderService(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Order obj)
        {
            var objFromDb = _db.Orders.FirstOrDefault(x => x.OrderId == obj.OrderId);
            if (objFromDb != null)
            {
                _db.Orders.Update(obj);
            }
        }

        public void UpdateStripePaymentId(int orderId, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.Orders.FirstOrDefault(x => x.OrderId == orderId);
            if(orderFromDb != null)
            {
                orderFromDb.SessionId = sessionId;
                orderFromDb.PaymentIntentId = paymentIntentId;
            }
        }
    }
}
