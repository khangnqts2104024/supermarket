using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Services
{
    public class OrderDetailService : Repository<OrderDetail>, IOrderDetail
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderDetail obj)
        {
            throw new NotImplementedException();
        }
    }
}
