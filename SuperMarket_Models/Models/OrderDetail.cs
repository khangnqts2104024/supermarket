
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int Count { get; set; }
        public decimal Price { get; set; }
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        [ValidateNever]
        public Order Order { get; set; }

        //public int ProductID { get; set; }
        //[ForeignKey("ProductID")]
       //[ValidateNever]
        //public Product Product { get; set; }
    }
}
