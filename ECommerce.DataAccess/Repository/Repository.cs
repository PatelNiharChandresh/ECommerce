using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
     public class Repository<T> : IRepository<T> where T : class
    {
        public ApplicationDbContext _context;
        public DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public void Add(T item)
        {
            _dbSet.Add(item);
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = _dbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = _dbSet;
            return query.ToList();
        }

        public void Remove(T item)
        {
            _dbSet.Remove(item);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            _dbSet.RemoveRange(items);
        }
    }
}
