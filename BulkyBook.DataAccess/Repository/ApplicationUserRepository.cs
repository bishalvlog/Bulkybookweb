using BulkyBook.Models;
using Bulkybookweb.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepositor
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUserRepository(ApplicationDbContext context) :base(context)
        {
            _context = context; 
        }
    }
}
