using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Bulkybookweb.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        public ApplicationDbContext _context;
        
        public OrderDetailsRepository(ApplicationDbContext context) :base(context)
        {
            _context = context; 
        }

        public void update(OrderDetails orderDetails)
        {
            _context.orderDetails.Update(orderDetails);
        }
    }
}
