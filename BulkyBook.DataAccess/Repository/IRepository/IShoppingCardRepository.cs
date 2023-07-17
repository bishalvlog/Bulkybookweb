﻿using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{

    public interface IShoppingCardRepository : IRepository<ShoppingCard>
    {

        int IncreamentCount (ShoppingCard shoppingCard, int count);
        int DecrementCount(ShoppingCard shoppingCard, int count);
      
    }
}
