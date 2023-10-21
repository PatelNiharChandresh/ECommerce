using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        private IProductRepository productRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IProductRepository repo)
        {
            this.productRepository = repo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = productRepository.GetAll(includeProperties:"Category");
            return View(products);
        }

        public IActionResult Details(int id)
        {
            Product product = productRepository.Get(u=>u.Id == id, includeProperties:"Category");
            return View(product);
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