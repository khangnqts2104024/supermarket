using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket_Models.Models
{

    public class Customer:IdentityUser
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int CustomerId { get; set; }
        
        [Required]
        [StringLength(100)]
        [DisplayName("Customer Name")]
        public string FullName { get; set; }


        [Required]
        [StringLength(150)]
        public string Address { get; set; }
		[StringLength(100)]
		public string CustomerAvatar { get; set; }
        [Required]
        [StringLength(50)]
        public string City { get; set; }
        [Required]
        [StringLength(50)]
        public string Country { get; set; }


    }
}