using BulkyBook.Models;
using Bulkybookweb.DataAccess;

namespace BulkyBook.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _context;

        public CategoryRepository (ApplicationDbContext context) :base(context)
        {
            _context = context; 
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Category category)
        {
            _context.categories.Update(category);
        }
    }
}
