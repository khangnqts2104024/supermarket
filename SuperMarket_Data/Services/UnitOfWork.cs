using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_DataAccess.Services
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IShoppingCart ShoppingCart { get;private set; }
        public IOrder Order { get;private set; }
        public IOrderDetail OrderDetail { get;private set; }
        public IProduct Product { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCart = new ShoppingCartService(_db);
            Order = new OrderService(_db);
            OrderDetail = new OrderDetailService(_db);
            Product = new ProductService(_db);
        }
        public async virtual Task<int> Save()
        {
            return await _db.SaveChangesAsync();
        }
      
        public void ClearTracking()
        {
            _db.ChangeTracker.Clear();
        }

       
    }
}
