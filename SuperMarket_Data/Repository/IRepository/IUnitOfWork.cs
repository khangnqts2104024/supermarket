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

<<<<<<< Updated upstream
=======
<<<<<<< HEAD
        void Save();
=======
>>>>>>> Stashed changes
        ICategory Category { get; }
        IBranch Branch { get; }
        IBrand Brand { get; }
        IStock Stock { get; }
        ICoupon Coupon { get; }
        ICustomer Customer { get; }
        Task<int> Save();
>>>>>>> 84651e2edcd6f53514780a71ae09b5877b145205

        void ClearTracking();
    }
}
