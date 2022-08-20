using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IShoppingCart ShoppingCart { get; }
        IOrder Order { get; }
        IOrderDetail OrderDetail { get; }
        void Save();
        void ClearTracking();
    }
}
