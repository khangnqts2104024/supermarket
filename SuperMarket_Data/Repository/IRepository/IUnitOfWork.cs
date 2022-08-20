using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IImageProduct ImageProduct { get; }
        IProduct Product { get; }
        IShoppingCart ShoppingCart { get; }
        IOrder Order { get; }
        IOrderDetail OrderDetail { get; }
        IProduct Product { get; }

        Task<int> Save();

        void ClearTracking();
    }
}
