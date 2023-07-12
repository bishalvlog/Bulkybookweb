using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Bulkybookweb.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public ApplicationDbContext _context;
        
        public CompanyRepository (ApplicationDbContext context) :base(context)
        {
            _context = context; 
        }

        public void update(Company company)
        {
            _context.companys.Update(company);
        }
        public void Delete(Company company)
        {
            _context.companys.Remove(company);
        }
    }
}
