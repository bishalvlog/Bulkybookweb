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
    public class ShoppingCardRepository : Repository<ShoppingCard>, IShoppingCardRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCardRepository(ApplicationDbContext context) :base(context)
        {
            _context = context;

        }

        public int DecrementCount(ShoppingCard shoppingCard, int count)
        {
            shoppingCard.Count -= count;
            return shoppingCard.Count;
        }

        public int IncreamentCount(ShoppingCard shoppingCard, int count)
        {
            shoppingCard.Count += count;
            return shoppingCard.Count;  
        }
    }
}
