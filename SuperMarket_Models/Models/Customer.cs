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
        public string FirstName { get; set; }


        [Required]
        public string LastName { get; set; }


        [Required]
        public string Address { get; set; }


        [Required]
        public string Street { get; set; }


        [Required]
        public string City { get; set; }


        [Required]
        public string Country { get; set; }


        [Required]
        public string Phone { get; set; }


        [Required]
        public string CreditCardNumber { get; set; }


        [Required]
        public DateTime CreditCardExpires { get; set; }
    }
}