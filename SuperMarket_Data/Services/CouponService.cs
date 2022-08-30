﻿using SuperMarket_DataAccess.Data;
using SuperMarket_DataAccess.Repository.IRepository;
using SuperMarket_DataAccess.Services.Generic_Imp;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Services
{
    public class CouponService:Repository<Coupon>,ICoupon
    {
        private readonly ApplicationDbContext _db;
        public CouponService(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(Coupon obj)
        {
            throw new NotImplementedException();
        }
    }
}
