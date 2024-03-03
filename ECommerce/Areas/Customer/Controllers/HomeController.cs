using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        private IProductRepository productRepository;
        private IShoppingCartRepository scRepo;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IProductRepository repo, IShoppingCartRepository shoppingCart)
        {
            this.productRepository = repo;
            this.scRepo = shoppingCart;
            _logger = logger;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = productRepository.GetAll(includeProperties:"Category");
            return View(products);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Product = productRepository.Get(u => u.Id == id, includeProperties: "Category"),
                Count = 1,
                ProductId = id
            };
           
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart) {
            

            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCart cartFromDb = scRepo.Get(u => u.ProductId == cart.ProductId && u.UserId == userId);

            if(cartFromDb != null)
            {
                //cart already exist, just increase count and update
                cartFromDb.Count += cart.Count;
                scRepo.Update(cartFromDb);
                scRepo.Save();
                TempData["success"] = "Count updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //this is a new cart entry, create a new entry in database
                cart.Id = 0;
                cart.UserId = userId;
                scRepo.Add(cart);
                scRepo.Save();
                TempData["success"] = "Added to cart successfully.";
                return RedirectToAction(nameof(Index));
            }

            
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}