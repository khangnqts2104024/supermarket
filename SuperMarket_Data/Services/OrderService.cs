using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Services
{
    public class OrderService : Repository<Order>, IOrder
    {
        private readonly ApplicationDbContext _db;

        public OrderService(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var objFromDb = _db.Orders.FirstOrDefault(x => x.OrderId == id);
            if (objFromDb != null)
            {
                objFromDb.OrderStatus = orderStatus;
                if(paymentStatus != null)
                {
                    objFromDb.PaymentStatus = paymentStatus;
                }
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
