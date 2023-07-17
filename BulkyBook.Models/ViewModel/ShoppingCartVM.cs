using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModel
{
    public class ShoppingCartVM
    {
        public  IEnumerable<ShoppingCard> ListCart { get; set; }
        public double cardTotal { get; set; }   
    }
}
