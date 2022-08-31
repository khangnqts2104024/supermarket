﻿using SuperMarket_DataAccess.Repository.IRepository.GenericInterface;
using SuperMarket_Models.Models;


namespace SuperMarket_DataAccess.Repository.IRepository
{
    public interface IBranch: IRepository<Branch>
    {
        void Update(Branch obj);
    }
}
