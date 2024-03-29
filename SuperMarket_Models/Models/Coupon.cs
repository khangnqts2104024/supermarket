﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CouponId { get; set; }
        [Required]
        public string? CouponCode { get; set; }
        [Required]
        [Range(1,99)]
        public int DiscountPercent { get; set; }
  
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime? ExpiredDate { get; set; }
        [Required]
        public int? Count { get; set; }

        [Required]
        public string Description { get; set; }
        [NotMapped]
        public virtual List<Product>? Products { set; get; }
    }
}
