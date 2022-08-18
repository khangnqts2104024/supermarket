using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarket_Models.Models
{
    [Table("tbProducts")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ProductId { get; set; }
        [Required]

        public string ProductName { get; set; }
        [Required]
        [Range(0,9999999999999.99)]
        public decimal Price { get; set; }
        [Required]
        public double TotalAmount { get; set; }
        [Required]

        public string Origin { get; set; }
        [Required]

        public string Title { get; set; }
        [Required]

        public string Description { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime ManufactureDate { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public double Weight { get; set; }
        public int CategoriesId { get; set; }
        [ForeignKey("CategoriesId")]
        [ValidateNever]
        public Category category { get; set; }
    }
}
