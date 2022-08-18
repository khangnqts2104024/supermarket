using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperMarket_Models.Models
{

    public class Customer
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


        [Required]
        [StringLength(50)]
        public string Phone { get; set; }
    }
}