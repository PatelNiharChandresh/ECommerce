using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public double OrderTotal { get; set; }

        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

    }
}
