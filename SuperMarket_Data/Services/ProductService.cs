using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Services
{
    public class ProductService : Repository<Product>, IProduct
    {
        private readonly ApplicationDbContext _db;
        public ProductService(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(x => x.ProductId == obj.ProductId);
            if(objFromDb != null)
            {

                objFromDb.ProductName = obj.ProductName;
                objFromDb.Price = obj.Price;
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.CreatedDate = obj.CreatedDate;
                objFromDb.ManufactureDate = obj.ManufactureDate;
                objFromDb.ExpiryDate = obj.ExpiryDate;
                objFromDb.Weight = obj.Weight;
                obj.Brand_Category = obj.Brand_Category;
                _db.Products.Update(objFromDb);
            }
        }

       
    }
}
