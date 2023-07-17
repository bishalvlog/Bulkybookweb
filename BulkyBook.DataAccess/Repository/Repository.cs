using BulkyBook.DataAccess.Repository.IRepository;
using Bulkybookweb.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository
{
    public  class Repository <T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbset;
        public Repository(ApplicationDbContext context)
        {
            _context = context; 
            //_context.products.Include(u=>u.Category).Include(u=>u.CoverType);
            this.dbset= _context.Set<T>();   
        }
        public  T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbset;
            query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var includProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))

                {
                    query = query.Include(includProp);

                }
            }

            return query.FirstOrDefault();
        }
        //include properties - "Category,CoverType"
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if(filter != null)
            {
                query = query.Where(filter);

            }
         
            if (includeProperties != null)
            {
                foreach (var includprop in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))

                {
                    query = query.Include(includprop);

                }
            }
            return query.ToList();  
        }

        public void Add(T entity)
        {
            dbset.Add(entity);
           
        }

        public void Remove(T entity)
        {
            dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
           dbset.RemoveRange(entity);
        }
    }
}
