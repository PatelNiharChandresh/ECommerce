using ECommerceRazor_Temp.Data;
using ECommerceRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceRazor_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
    {

        public ApplicationDbContext _context;

        [BindProperty]
        public Category Category { get; set; }
        public DeleteModel(ApplicationDbContext context) {
            _context = context;
        }
        public void OnGet(int? Id)
        {

            if (Id != null)
            {
                Category = _context.Categories.Find(Id);
            }

        }

        public IActionResult OnPost()
        {
            Category obj = _context.Categories.Find(Category.Id);
            if(obj == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(obj);
            _context.SaveChanges();
            TempData["success"] = "Data deleted successfully";
            return RedirectToPage("/categories/index");
        }
    }
}
