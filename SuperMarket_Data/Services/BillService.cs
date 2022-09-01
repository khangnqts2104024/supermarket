using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Services
{
    public class BillService : Repository<Bill>, IBill
    {
        private readonly ApplicationDbContext _db;
        public BillService(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Bill obj)
        {
            var objFromDb = _db.Bills.FirstOrDefault(x => x.BillId == obj.BillId);
            if(objFromDb != null)
            {
                objFromDb.BillAmount = obj.BillAmount;
                objFromDb.CreatedDate = obj.CreatedDate;

                _db.Bills.Update(objFromDb);
            }
        }
    }
}
