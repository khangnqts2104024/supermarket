using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    public class Brand_Category
    {
        [Required]
        public int BrandId { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
