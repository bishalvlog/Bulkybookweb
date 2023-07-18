using BulkyBook.Models;
using Bulkybookweb.DataAccess;

namespace BulkyBook.DataAccess.Repository
{
    public class OrderHeadRepository : Repository<orderHead>, IOrderHeadRepository
    {
        private ApplicationDbContext _context;

        public OrderHeadRepository(ApplicationDbContext context) :base(context)
        {
            _context = context; 
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(orderHead orderHead)
        {
            _context.orderheads.Update(orderHead);
        }

        public void UpdateStatus(int id, string OrderStatus, string? PaymentStatus = null)
        {
            var orderdbfrom = _context.orderheads.FirstOrDefault(o => o.Id == id);
            if (orderdbfrom != null)
            {
                orderdbfrom.OrderStatus = OrderStatus;
                if(PaymentStatus != null)
                {
                    orderdbfrom.PaymentsStatus = PaymentStatus; 
                }
            }
        }
    }
}
