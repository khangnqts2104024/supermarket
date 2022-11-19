using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Services
{
    public class Brand_CategoryService : Repository<Brand_Category>, IBrand_Category
    {
        private readonly ApplicationDbContext _db;
        public Brand_CategoryService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Brand_Category obj)
        {
            var objFromDb = _db.Brand_Categories.FirstOrDefault(x => x.BrandCateId == obj.BrandCateId);
            if (objFromDb != null)
            {

                objFromDb.CategoryId = obj.CategoryId;
                objFromDb.BrandId = obj.BrandId;
                _db.Brand_Categories.Update(objFromDb);
            }
        }
    }
}
