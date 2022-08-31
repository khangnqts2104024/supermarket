using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket_Models.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public decimal OrderTotal { get; set; }
        [Required]
        public string? OrderStatus { get; set; }
        [Required]
        public string? PaymentStatus { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        public string? OrderNotes { get; set; }

        public int? CouponId { get; set; } = null;
        [ForeignKey("CouponId")]
        [ValidateNever]
        public Coupon? Coupon { get; set; }
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        [ValidateNever]
        public Customer Customer { get; set; }
        [ValidateNever]
        public List<OrderDetail> OrderDetail { get; set; }
    }
}