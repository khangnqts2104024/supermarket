using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Services
{
    public class CustomerService:Repository<Customer>,ICustomer
    {
        private readonly ApplicationDbContext _db;
        public CustomerService(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Customer obj)
        {
            var objFromDb = _db.Customers.FirstOrDefault(x => x.Id == obj.Id);
            if (objFromDb != null)
            {

                objFromDb.FullName = obj.FullName;
                objFromDb.PhoneNumber = obj.PhoneNumber;
                objFromDb.Address = obj.Address;
                objFromDb.Email = obj.Email;
                objFromDb.UserName = obj.UserName;
				objFromDb.CustomerAvatar = obj.CustomerAvatar;
                objFromDb.City = obj.City;
                objFromDb.Country = obj.Country;
                objFromDb.CustomerAvatar = obj.CustomerAvatar;
				_db.Customers.Update(objFromDb);
            }

        }
    }
}
