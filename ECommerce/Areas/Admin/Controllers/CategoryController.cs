
using ECommerce.DataAccess.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository repo
            ;

        public CategoryController(ICategoryRepository repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            List<Category> myList = repo.GetAll().ToList();
            return View(myList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                repo.Add(obj);
                repo.Save();
                TempData["success"] = "Category Created Successfully.";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Edit(int? Id)
        {
            if (Id != null && Id != 0)
            {
                Category? category = repo.Get(u => u.Id == Id);
                return View(category);
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                repo.Update(obj);
                repo.Save();
                TempData["success"] = "Category Updated Successfully.";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Delete(int? Id)
        {
            if (Id != 0 && Id != null)
            {
                Category? category = repo.Get(u => u.Id == Id);
                return View(category);
            }
            else
            {
                return View();
            }

        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteObject(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }

            Category? category = repo.Get(u => u.Id == Id);
            if (category == null)
            {
                return NotFound();
            }
            repo.Remove(category);
            repo.Save();
            TempData["success"] = "Category Deleted Successfully.";
            return RedirectToAction("Index");

        }
    }
}
