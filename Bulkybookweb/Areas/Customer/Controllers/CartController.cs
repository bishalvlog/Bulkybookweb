using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using System.Drawing.Printing;
using System.Security.Claims;

namespace Bulkybookweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController (IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
            };
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price =GetPriceBaseOnQuantity(cart.Count,cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.cardTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Plus (int cartId)
        {
            var cart = _unitOfWork.ShoppingCard.GetFirstOrDefault(u=>u.Id ==cartId);
            _unitOfWork.ShoppingCard.IncreamentCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            return View();
        }
        public IActionResult Minus (int cartId)
        {
            var cart = _unitOfWork.ShoppingCard.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCard.Remove(cart);
            }
            else
            {
                _unitOfWork.ShoppingCard.DecrementCount(cart, 1);
               
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove (int cartId)
        {
            var cart = _unitOfWork.ShoppingCard.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.ShoppingCard.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index)); 
        }

        private double GetPriceBaseOnQuantity(double quantity, double price , double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else
            {
                if(quantity <= 100)
                {
                    return price50;
                }
                return price100;
            }

        }
    }
}
