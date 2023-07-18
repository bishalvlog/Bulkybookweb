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
                ListCart = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                orderHead = new ()
            };
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price =GetPriceBaseOnQuantity(cart.Count,cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.orderHead.OrderTotal += (cart.Price * cart.Count);
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value,
                includeProperties: "Product"),
                orderHead = new()
            };
            ShoppingCartVM.orderHead.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault
                (u => u.Id == claim.Value);

            ShoppingCartVM.orderHead.Name = ShoppingCartVM.orderHead.ApplicationUser.Name;
            ShoppingCartVM.orderHead.PhoneNumber = ShoppingCartVM.orderHead.ApplicationUser.PhoneNumber;
            ShoppingCartVM.orderHead.StreetAddress = ShoppingCartVM.orderHead.ApplicationUser.StreetAddress;
            ShoppingCartVM.orderHead.City = ShoppingCartVM.orderHead.ApplicationUser.City;
            ShoppingCartVM.orderHead.State = ShoppingCartVM.orderHead.ApplicationUser.State;
            ShoppingCartVM.orderHead.PostalCode = ShoppingCartVM.orderHead.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.orderHead.OrderTotal += (cart.Price * cart.Count);
            }
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
