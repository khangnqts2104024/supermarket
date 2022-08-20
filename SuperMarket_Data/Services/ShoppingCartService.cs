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
    public class ShoppingCartService : Repository<ShoppingCart>, IShoppingCart
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int DecrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count -= count;
            return shoppingCart.Count;
        }

        public int IncrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }

        public void Update(ShoppingCart obj, int count, string action)
        {
            var cartFromDb = _db.ShoppingCarts.FirstOrDefault(x => x.CartId == obj.CartId);
            
           if(cartFromDb != null)
            {
                if (action == "Increment")
                {
                    cartFromDb.Count += count;

                }
                else if (action == "Decrement")
                {
                    cartFromDb.Count -= count;
                }
                _db.ShoppingCarts.Update(cartFromDb);
            }
        }
    }
}
