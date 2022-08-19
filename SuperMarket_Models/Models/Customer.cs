using Microsoft.AspNetCore.Identity;
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
        public string FullName { get; set; }


        [Required]
        [StringLength(150)]
        public string Address { get; set; }

    }
}