using SuperMarket_DataAccess.Repository.IRepository.GenericInterface;
using SuperMarket_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_DataAccess.Repository.IRepository
{
    public interface IStock: IRepository<Stock>
    {

        void IncrementStock(Stock obj,int count);
        void DecrementStock(Stock obj,int count);
    }
}
