using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using ECommerce.Models.Models;
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
    public class CompanyController : Controller
    {

        public ICompanyRepository repo;
        public ICategoryRepository categoryRepository;

        public CompanyController(ICompanyRepository repo, ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
            this.repo = repo;
        }
        public IActionResult Index()
        {
            var Companys = repo.GetAll();

            return View(Companys);
        }

        public IActionResult Upsert(int? id)
        {

            if(id == null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                Company company = repo.Get(u => u.Id ==  id);
                return View(company);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {

            if (ModelState.IsValid)
            {
               
                    if (company.Id == 0)
                    {
                        repo.Add(company);
                    }


                    if (company.Id != 0)
                    {
                        repo.Update(company);
                    }
                
                repo.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View(company);
            }
            
        }

        public IActionResult Edit(int? id)
        {
            if(id != 0 && id != null)
            {
                Company Company = repo.Get(u  => u.Id == id);
                if(Company != null)
                {
                    return View(Company);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(Company Company)
        {
            if (ModelState.IsValid)
            {
                repo.Update(Company);
                repo.Save();
                return RedirectToAction("Index");
            }
            return View();
        } 

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteCompany(int? id)
        {
            if(id != 0 && id != null)
            {
                Company Company = repo.Get(u =>u.Id == id);
                if (Company != null)
                {
                    repo.Remove(Company);
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
            var Companys = repo.GetAll();

            return Json(new { data = Companys });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToDelete = repo.Get(u=> u.Id == id);

            if (CompanyToDelete == null)
            {
                return Json(new { success = false , message = "Error while deleting." });
            }
           

            repo.Remove(CompanyToDelete); 
            repo.Save();

            return Json(new { success = true, message = "Deleted successfully." });
        }

        #endregion

    }
}
