using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public interface IOrderHeadRepository :IRepository<orderHead>
    {
        void Update (orderHead orderHead);
        void UpdateStatus(int id, string OrderStatus, string? PaymentStatus = null);

    }
}
