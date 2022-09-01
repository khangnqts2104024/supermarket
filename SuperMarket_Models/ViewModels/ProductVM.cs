using SuperMarket_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.ViewModels
{
    public class ProductVM
    {
        public Stock? stockobj { get; set; }
        public Product? productobj { get; set; }
        public Category? categoryobj { get; set; }
        public Brand? brandobj { get; set; }
    }
}
