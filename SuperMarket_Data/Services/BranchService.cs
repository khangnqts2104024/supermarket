﻿using SuperMarket_DataAccess.Data;
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
            throw new NotImplementedException();
        }
    }
}
