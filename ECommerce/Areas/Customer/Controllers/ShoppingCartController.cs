using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace ECommerce.Areas.Customer.Controllers
{
    [Area(nameof(Customer))]
    public class ShoppingCartController : Controller
    {
        private IShoppingCartRepository repo;
        //public ShoppingCartVM shoppingCartVM { get; set; }
        public ShoppingCartController(IShoppingCartRepository sc) {
            repo = sc;
        }
        
        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var uId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM scVM = new()
            {
                ShoppingCartList = repo.GetAll(u => u.UserId == uId, includeProperties : "Product")
            };

            return View(scVM);
        }

    }


}
