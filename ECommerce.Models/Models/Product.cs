using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models
{
    
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Range(0, 1000)]
        [Required]
        public double Price { get; set; }
        
        public int CategoryId { get; set; }

        /*[ValidateNever]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }*/

        [ValidateNever]
        public string ImageURL { get; set; }
    } 
}
