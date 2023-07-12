using BulkyBook.Models;
using Bulkybookweb.DataAccess;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _context;

        public ProductRepository (ApplicationDbContext context) :base(context)
        {
            _context = context; 
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            var productfromdb = _context.products.FirstOrDefault(u=>u.Id==product.Id);
            if (productfromdb != null) 
            { 
                productfromdb.Title = product.Title;    
                productfromdb.Description = product.Description;    
                productfromdb.Category = product.Category;  
                productfromdb.Price = product.Price;    
                productfromdb.Price50 = product.Price50;    
                productfromdb.Price100 = product.Price100;
                productfromdb.ListPrice = product.ListPrice;
                productfromdb.CategoryId = product.CategoryId;  
                productfromdb.CoverType = product.CoverType;    
                productfromdb.Author = product.Author;  
                if(product.ImageUrl != null) 
                {
                    productfromdb.ImageUrl = product.ImageUrl;  
                }


            }

        }
    }
}
