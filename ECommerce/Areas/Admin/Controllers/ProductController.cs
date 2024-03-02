using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {

        public IProductRepository repo;
        public ICategoryRepository categoryRepository;
        private IWebHostEnvironment env;

        public ProductController(IProductRepository repo, ICategoryRepository categoryRepository, IWebHostEnvironment env)
        {
            this.categoryRepository = categoryRepository;
            this.repo = repo;
            this.env = env;
        }
        public IActionResult Index()
        {
            var products = repo.GetAll(includeProperties:"Category");

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
        public IActionResult Upsert(ProductVM productVM, IFormFile? formFile)
        {

            if (ModelState.IsValid)
            {


                if (formFile != null)
                {
                    string pathToWWWRoot = env.WebRootPath;
                    string completePath = Path.Combine(pathToWWWRoot, @"images\product");
                    string completeFileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);

                    if (!string.IsNullOrEmpty(productVM.Product.ImageURL))
                    {
                        string oldPath = Path.Combine(pathToWWWRoot, productVM.Product.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(completePath, completeFileName), FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                    productVM.Product.ImageURL = @"\images\product\" + completeFileName;
                }
               
                    if (productVM.Product.Id == 0)
                    {
                        repo.Add(productVM.Product);
                    }


                    if (productVM.Product.Id != 0)
                    {
                        repo.Update(productVM.Product);
                    }
                
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

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = repo.GetAll(includeProperties:"Category");

            return Json(new { data = products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToDelete = repo.Get(u=> u.Id == id);

            if (productToDelete == null)
            {
                return Json(new { success = false , message = "Error while deleting." });
            }
            var oldPath = Path.Combine(env.WebRootPath, productToDelete.ImageURL.TrimStart('\\'));
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            repo.Remove(productToDelete); 
            repo.Save();

            return Json(new { success = true, message = "Deleted successfully." });
        }

        #endregion

    }
}
