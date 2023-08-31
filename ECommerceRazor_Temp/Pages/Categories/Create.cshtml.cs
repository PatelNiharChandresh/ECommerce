using ECommerceRazor_Temp.Data;
using ECommerceRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceRazor_Temp.Pages.Categories
{
    public class CreateModel : PageModel
    {

        public ApplicationDbContext _context;

        [BindProperty]
        public Category Category { get; set; }
        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            _context.Categories.Add(Category);
            _context.SaveChanges();
            TempData["success"] = "Data inserted successfully";
            return RedirectToPage("/categories/index");
        }
    }
}
