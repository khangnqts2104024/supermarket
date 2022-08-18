using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    public class Bill
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillID { get; set; }
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public Order Order { get; set; }

        //public int CustomerID { get; set; }
        //[ForeignKey("CustomerID")]
        //[ValidateNever]
        //public Customer Customer { get; set; }

        public decimal BillAmount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
