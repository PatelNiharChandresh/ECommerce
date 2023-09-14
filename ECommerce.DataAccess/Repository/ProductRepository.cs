using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context): base(context)
        {
            _context = context;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        
        public void Update(Product product)
        {
            var existingProduct = _context.Products.FirstOrDefault(u => u.Id == product.Id);
            
            if (existingProduct != null)
            {
                existingProduct.ISBN = product.ISBN;
                existingProduct.Title = product.Title;
                existingProduct.Description = product.Description;
                existingProduct.Author = product.Author;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;
                if(product.ImageURL != null)
                {
                    existingProduct.ImageURL = product.ImageURL;
                }
            }
        }
    }
}
