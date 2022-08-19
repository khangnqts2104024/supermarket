using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    public class Brand_Category
    {
        [Column(Order = 0), Key, ForeignKey("Brand")]
        public int BrandId { get; set; }

        [Column(Order = 1), Key, ForeignKey("Category")]
        public int CategoryId { get; set; }
        
        public virtual Brand Brand { get; set; }
        public virtual Category Category { get; set; }
    }
}
