
using ECommerce.Models;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<ApplicationUser> applicationUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder); 
            
            modelBuilder.Entity<Category>().HasData(new Category { Id = 1 , Name = "Books", DisplayOrder = 3},
                new Category { Id = 2, Name = "Clothes", DisplayOrder = 1 },
                new Category { Id = 3, Name = "Shoes", DisplayOrder = 2 });

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 4, Title = "Book1", Author = "Ram", Description = "sdklfshdnfjlksdh", ISBN = "123ABCD", Price = 14.25, CategoryId = 1, ImageURL="" },
                new Product { Id = 5, Title = "Book2", Author = "Shyam", Description = "sdklfshdnfjlksdh", ISBN = "456EFG", Price = 15.20, CategoryId = 3, ImageURL = "" },
                new Product { Id = 6, Title = "Book3", Author = "Sita", Description = "sdklfshdnfjlksdh", ISBN = "789HIJK", Price = 16.33, CategoryId = 2, ImageURL = "" },
                new Product { Id = 7, Title = "Book4", Author = "Lakshman", Description = "sdklfshdnfjlksdh", ISBN = "1768AEF", Price = 10.11, CategoryId = 2, ImageURL = "" },
                new Product { Id = 8, Title = "Book5", Author = "Ganesh", Description = "sdklfshdnfjlksdh", ISBN = "986KJLGH", Price = 60.30, CategoryId = 3, ImageURL = "" }
                );
        }


    }

    

}
