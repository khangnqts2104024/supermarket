using SuperMarket_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.ViewModels
{
    public class ShoppingCartVM
    {
        public List<ShoppingCart> ListCart { get; set; }
        public Order Order { get; set; }
        public Coupon Coupon { get; set; }
        
    }
}
