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
    public class Stock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockId { get; set; }
        public int? Count { get; set; }
      
        //FK
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }

        //FK
        public int? BranchId { get; set; }
        [ForeignKey("BranchId")]
        [ValidateNever]
        public Branch? Branch { get; set; }

        [NotMapped]
        [ValidateNever]
        public int RatingPointAverage { get; set; }
    }
}
