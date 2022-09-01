using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Services
{
    public class BrandService:Repository<Brand>,IBrand
    {


        private readonly ApplicationDbContext _db;


     

        public BrandService(ApplicationDbContext db) : base(db)
        {
            _db= db;
        }

        public void Update(Brand obj)
        {
            var objFromDb = _db.Brands.FirstOrDefault(x => x.BrandId == obj.BrandId);
            if (objFromDb != null)
            {

                objFromDb.BrandName = obj.BrandName;
                objFromDb.Origin = obj.Origin;
                objFromDb.Phone = obj.Phone;
                objFromDb.Address = obj.Address;
                objFromDb.UpdateDate = obj.UpdateDate;

                _db.Brands.Update(objFromDb);
            }
        }
    }
}
