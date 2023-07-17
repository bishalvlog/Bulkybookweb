using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Bulkybookweb.DataAccess
{
    public class ApplicationDbContext :IdentityDbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }

       public DbSet<Category> categories {  get; set; }  
        public DbSet<CoverType> coverTypes { get; set; } 
        public DbSet<Product> products { get; set; }   
        public DbSet<ApplicationUser> Applicationusers { get; set; }
        public DbSet<Company> companys { get; set; }  
        public DbSet<ShoppingCard> shoppingcards { get; set;}
    }
}
