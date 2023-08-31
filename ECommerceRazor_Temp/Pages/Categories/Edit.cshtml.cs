using ECommerceRazor_Temp.Data;
using ECommerceRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceRazor_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {

        public ApplicationDbContext _context;

        [BindProperty]
        public Category Category { get; set; }

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet(int? Id)
        {
            if(Id != null)
            {
                Category = _context.Categories.Find(Id);
            }
            
        }

        public IActionResult OnPost()
        {

            if(ModelState.IsValid)
            {
                    _context.Categories.Update(Category);
                    _context.SaveChanges();
                TempData["success"] = "Data updated successfully";
                return RedirectToPage("/categories/index");
                
            }
            return Page();
        }
    }
}
