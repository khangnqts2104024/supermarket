using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SuperMarket_Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.ViewModels
{
    public class Cart_Feedback_RatingVM
    {
        public int Count { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        [ValidateNever]
        public Customer Customer { get; set; }


        [NotMapped]
        [ValidateNever]
        public List<Feedback_Rating> Feedback_RatingList { get; set; }
        

        [NotMapped]
        [ValidateNever]
        public int StockCount { get; set; }

        [NotMapped]
        [ValidateNever]
        public int branchId { get; set; }

        [NotMapped]
        [ValidateNever]
        public int FeedbackCount { get; set; }

        [NotMapped]
        [ValidateNever]
        public int RatingPointAverage { get; set; }
    }
}
