using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    [Table("tbBranch")]
    public class Branch
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int BranchId { get; set; }

        public string? BranchName { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Phone { get; set; }



    }
}
