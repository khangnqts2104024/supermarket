using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Services
{
    public class BranchService :Repository<Branch>,IBranch
    {
     
        private readonly ApplicationDbContext _db;
   

        public BranchService(ApplicationDbContext db):base(db)
        {
            _db=db;
        }

        public void Update(Branch obj)
        {
            var objFromDb = _db.Branches.FirstOrDefault(x => x.BranchId == obj.BranchId);
            if (objFromDb != null)
            {

                objFromDb.BranchName = obj.BranchName;
                objFromDb.Address = obj.Address;
                objFromDb.Phone = obj.Phone;
                objFromDb.Address = obj.Address;
                objFromDb.Latitude = obj.Latitude;
                objFromDb.Longtitude = obj.Longtitude;
                objFromDb.BranchImg=obj.BranchImg;
                _db.Branches.Update(objFromDb);
            }
        }
    }
}
