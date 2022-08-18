using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket_Models.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public decimal OrderTotal { get; set; }
        [Required]
        public string? OrderStatus { get; set; }
        [Required]
        public decimal Amount { get; set; }
  
        [Required]
        public string? PaymentStatus { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        [Required]
        public string? SessionId { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? Phone { get; set; }

        //public int CouponID { get; set; }
        //[ForeignKey("CouponID")]

        //public Coupon Coupon { get; set; }

        //public int CustomerID { get; set; }
        //[ForeignKey("CustomerID")]

        //public Customer Customer { get; set; }
    }
}