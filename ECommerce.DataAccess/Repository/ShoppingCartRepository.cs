using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>,IShoppingCartRepository 
    {
        ApplicationDbContext dbContext;
        public ShoppingCartRepository(ApplicationDbContext context) : base(context) {
            dbContext = context;
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }

        public void Update(ShoppingCart cart)
        {
            dbContext.ShoppingCarts.Update(cart);
        }
    }
}
