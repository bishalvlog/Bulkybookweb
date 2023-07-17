using BulkyBook.DataAccess.Repository.IRepository;
using Bulkybookweb.DataAccess;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;

        public UnitOfWork (ApplicationDbContext context) 
        {
            _context = context;
            Category = new CategoryRepository (_context);
            CoverType = new CoverTypeRepository (_context);
            Product = new ProductRepository (_context); 
            Company = new CompanyRepository (_context); 
            ApplicationUser = new ApplicationUserRepository (_context);
            ShoppingCard = new ShoppingCardRepository (_context);   
        }
        public ICategoryRepository Category { get; private set; }  
        public ICoverTypesRepository CoverType { get; private set; }
         public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IApplicationUserRepositor ApplicationUser { get; private set; }
        public IShoppingCardRepository ShoppingCard { get; private set; }


        public void Save()
        {
            _context.SaveChanges();
        }
      
    }
}
