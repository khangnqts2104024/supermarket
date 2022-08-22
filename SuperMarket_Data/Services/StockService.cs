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
   public class StockService :Repository<Stock>,IStock
    {
        private readonly ApplicationDbContext _db;
        public StockService(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void IncrementStock(Stock obj,int count)
        {

            var editStock = _db.Stocks.FirstOrDefault(s => s.StockId.Equals(obj.StockId));
            if (editStock != null && count >=0)
            {
                editStock.Count += count;
                _db.Stocks.Update(editStock);

            }
     

        }
        public void DecrementStock(Stock obj, int count)
        {

            var editStock = _db.Stocks.FirstOrDefault(s => s.StockId.Equals(obj.StockId));
            if (editStock != null && count >= 0 && editStock.Count>=count)
            {


                editStock.Count -= count;
                _db.Stocks.Update(editStock);

            }
          

        }

  
    }
}
