﻿using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;


namespace SuperMarket_DataAccess.Services
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IShoppingCart ShoppingCart { get;private set; }
        public IOrder Order { get;private set; }
        public IOrderDetail OrderDetail { get;private set; }
        public IProduct Product { get; private set; }
        public IImageProduct ImageProduct { get; private set; }
        public ICategory Category { get; private set; }
        public IBranch Branch { get; private set; }
        public IBrand Brand { get; private set; }
        public IStock Stock { get; private set; }
        public ICoupon Coupon { get; private set; }
        public ICustomer Customer { get; private set; }
        public IBrand_Category Brand_Category { get; private set; }
        public IFeedback_Rating Feedback_Rating { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCart = new ShoppingCartService(_db);
            Order = new OrderService(_db);
            OrderDetail = new OrderDetailService(_db);
            Product = new ProductService(_db);
            Category = new CategoryService(_db);
            Branch=new BranchService(_db);
            Brand = new BrandService(_db);
            Stock = new StockService(_db);
            Coupon = new CouponService(_db);
            Customer=new CustomerService(_db);
            ImageProduct = new ImageProductService(_db);
            Brand_Category = new Brand_CategoryService(_db);
            Feedback_Rating = new Feedback_RatingService(_db); 
        }
        public virtual async Task<int> Save()
        {
          return await _db.SaveChangesAsync();
        }
      
        public void ClearTracking()
        {
            _db.ChangeTracker.Clear();
        }
    }
}
