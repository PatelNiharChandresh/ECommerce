using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models.Models
{
    public class ShoppingCart
    {

        public int Id { get; set; }
        [Range(1,1000, ErrorMessage = "Please enter number between 1 to 1000")]
        public int Count { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        [ValidateNever]
        public Product Product { get; set; }
        
        
    }
}
