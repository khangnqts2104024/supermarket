using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket_Models.Models
{
    [Table("tbCustomers")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CustomerId { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        [Required]
        public string LastName { get; set; }
        [StringLength(50)]
        [Required]
        public string Street { get; set; }
        [StringLength(50)]
        [Required]
        public string City { get; set; }
        [StringLength(50)]
        [Required]
        public string Country { get; set; }
        [StringLength(50)]
        [Required]
        public string Phone { get; set; }
        [StringLength(50)]
        [Required]
        public string CreditCardNumber { get; set; }
        [StringLength(50)]
        [Required]
        public DateTime CreditCardExpires { get; set; }
        [Required]
        public string Ward { get; set; }
        [StringLength(50)]
        [Required]
        public string District { get; set; }
    }
}