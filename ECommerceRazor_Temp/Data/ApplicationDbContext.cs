using ECommerceRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceRazor_Temp.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Category[] categorySeed =
            {
                new Category { Id = 1, Name = "Books", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Shoes" , DisplayOrder = 2 },
                new Category { Id = 3, Name = "Clothes", DisplayOrder=3 }
            };

            modelBuilder.Entity<Category>().HasData(categorySeed);

        }
    }
}

