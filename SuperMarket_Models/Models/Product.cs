using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace SuperMarket_Models.Models
{
    //[JsonObject(IsReference = true)]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        [Range(0,9999999999999.99)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 9999999999999.99)]
        [NotMapped]
        public decimal FinalPrice { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }


        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime ManufactureDate { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        public string Weight { get; set; }
        public int BrandCateId { get; set; }
        [ForeignKey("BrandCateId")]
        [ValidateNever]
        public Brand_Category Brand_Category { get; set; }
   
        public virtual List<ImageProduct> ImageProduct { get; set; }

        public List<Stock>? Stock { get; set; }
        [ValidateNever]

        public virtual List<Feedback_Rating> Feedback_Ratings { get; set; }


    }
}
