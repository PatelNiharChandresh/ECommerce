using ECommerceRazor_Temp.Data;
using ECommerceRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ECommerceRazor_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {

        public ApplicationDbContext _context;
        public List<Category> _categories;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            _categories = _context.Categories.ToList();
        }
    }
}
