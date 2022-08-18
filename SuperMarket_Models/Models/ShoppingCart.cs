using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    public class ShoppingCart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartID { get; set; }
        [Required]
        public int Count { get; set; }
        //public int ProductID { get; set; }
        //[ForeignKey("ProductID")]
        //[ValidateNever]
        //public Product Product { get; set; }

        //public int CustomerID { get; set; }
        //[ForeignKey("CustomerID")]
        //[ValidateNever]
        //public Customer Customer { get; set; }
    }
}
