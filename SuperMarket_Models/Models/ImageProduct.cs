using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    public class ImageProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }
        [Required]
        public string Url { get; set; }
        public bool IsMainImage { get; set; }
        public int ProductId { get; set; }
      
        [ForeignKey("ProductId")]

        [ValidateNever]
        [JsonIgnore]
        public Product Product { get; set; }
    }
}
