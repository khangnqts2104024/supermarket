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
                //update product field anh tu ghi nha anh.

                _db.Products.Update(objFromDb);
            }
        }

       
    }
}
