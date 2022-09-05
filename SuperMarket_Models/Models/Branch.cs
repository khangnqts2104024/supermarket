using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{

    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int BranchId { get; set; }
        [MaxLength(150)]
        [Required]
        public string BranchName { get; set; }
        [MaxLength(250)]
        [Required]

        public string Address { get; set; }
        [MaxLength(20)]
        [Required]

        public string Phone { get; set; }
        [Required]

        public string Latitude { get; set; }
        [Required]

        public string Longtitude { get; set; }
        
        public string? BranchImg { get; set; }


    }
}
