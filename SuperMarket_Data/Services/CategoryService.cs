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
    public class CategoryService : Repository<Category>, ICategory
    {


        private readonly ApplicationDbContext _db;




        public CategoryService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category obj)
        {
            var objFromDb = _db.Categories.FirstOrDefault(x => x.CategoryId == obj.CategoryId);
            if (objFromDb != null)
            {

                objFromDb.CategoryName = obj.CategoryName;
                objFromDb.UpdateDate=obj.UpdateDate;
                _db.Categories.Update(objFromDb);
            }
        }
    }
}
