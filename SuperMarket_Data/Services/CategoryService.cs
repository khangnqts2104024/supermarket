﻿using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


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
                objFromDb.CategoryImg=obj.CategoryImg;
                _db.Categories.Update(objFromDb);
            }
        }
    }
}
