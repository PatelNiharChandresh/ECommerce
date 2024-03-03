using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
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
        private IProductRepository productRepository;
        //public ShoppingCartVM shoppingCartVM { get; set; }
        public ShoppingCartController(IShoppingCartRepository sc, IProductRepository productRepository)
        {
            this.productRepository = productRepository;
            repo = sc;
        }

        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var uId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM scVM = new()
            {
                ShoppingCartList = repo.GetAll(u => u.UserId == uId, includeProperties: "Product"),
                OrderTotal = CalculateOrderTotal(repo.GetAll(u => u.UserId == uId, includeProperties: "Product"))
            };

            return View(scVM);
        }


        public IActionResult Increase(int cartId) {

            var shoppingCart = repo.Get(u => u.Id == cartId);
            shoppingCart.Count += 1;
            repo.Update(shoppingCart);
            repo.Save();
            return RedirectToAction(nameof(Index));   

        }
        public IActionResult Decrease(int cartId) {

            var shoppingCart = repo.Get(u => u.Id == cartId);
            if (shoppingCart.Count <= 1)
            {
                repo.Remove(shoppingCart);
            }
            else
            {
                shoppingCart.Count -= 1;
                repo.Update(shoppingCart);
            }
            repo.Save();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Remove(int cartId) {
        
            var shoppingCart = repo.Get(u =>u.Id == cartId);
            repo.Remove(shoppingCart);
            repo.Save();
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Summary() { return View(); }


        private double CalculateOrderTotal(IEnumerable<ShoppingCart> shoppingCart)
        {
            double total = 0;
            if(shoppingCart != null)
            {
                foreach (var sc in shoppingCart)
                {
                    var product = productRepository.Get(u => u.Id == sc.ProductId);
                    var price = product.Price;
                    total += price * sc.Count;
                    return total;
                }
            }
            
            return 0;
        }
    }
}
