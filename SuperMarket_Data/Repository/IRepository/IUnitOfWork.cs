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
        ICategory Category { get; }
        IBranch Branch { get; }
        IBrand Brand { get; }
        IStock Stock { get; }
        ICoupon Coupon { get; }
        ICustomer Customer { get; }
        IBrand_Category Brand_Category { get; }
        Task<int> Save();
        void ClearTracking();
    }
}
