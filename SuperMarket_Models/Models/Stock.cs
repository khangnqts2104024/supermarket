using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    [Table("tbStock")]
    public class Stock
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StockId { get; set; }


        
        public int? Count { get; set; }

      

        //FK
        public int? ProductId { get; set; }

        //public Product Product { get; set; }

        //FK
        public int? BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}
