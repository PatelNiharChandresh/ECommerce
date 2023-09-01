using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        public IProductRepository repo;
        public ICategoryRepository categoryRepository;

        public ProductController(IProductRepository repo, ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
            this.repo = repo;
        }
        public IActionResult Index()
        {
            var products = repo.GetAll();

            return View(products);
        }

        public IActionResult Upsert(int? id)
        {

           ProductVM productsVM = new()
           {
               CategoryList = categoryRepository.GetAll().Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString()
               }),
               Product = new Product()
            };

            if(id == null || id == 0)
            {
                return View(productsVM);
            }
            else
            {
                productsVM.Product = repo.Get(u => u.Id ==  id);
                return View(productsVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile formFile)
        {
            

            if(ModelState.IsValid)
            {
                repo.Add(productVM.Product);
                repo.Save();
                return RedirectToAction("Index");
            }
            else
            {
                ProductVM productsVM = new()
                {
                    CategoryList = categoryRepository.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                    Product = new Product()
                };
                return View(productsVM);
            }
            
        }

        public IActionResult Edit(int? id)
        {
            if(id != 0 && id != null)
            {
                Product product = repo.Get(u  => u.Id == id);
                if(product != null)
                {
                    return View(product);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                repo.Update(product);
                repo.Save();
                return RedirectToAction("Index");
            }
            return View();
        } 

        public IActionResult Delete(int? id)
        {
            if(id != 0 && id != null)
            {
                Product product = repo.Get(u => u.Id == id);
                if(product != null)
                {
                    return View(product);
                }
            }
            return View();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteProduct(int? id)
        {
            if(id != 0 && id != null)
            {
                Product product = repo.Get(u =>u.Id == id);
                if (product != null)
                {
                    repo.Remove(product);
                    repo.Save();
                    return RedirectToAction("Index");
                }
            }
            return View();
        }
    }
}
