using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBooks.Utilitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bulkybookweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]

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
           
            ShoppingCartVM.orderHead.PhoneNumber = ShoppingCartVM.orderHead.ApplicationUser.PhoneNumber;
            ShoppingCartVM.orderHead.StreetAddress = ShoppingCartVM.orderHead.ApplicationUser.StreetAddress;
            ShoppingCartVM.orderHead.City = ShoppingCartVM.orderHead.ApplicationUser.City;
            ShoppingCartVM.orderHead.State = ShoppingCartVM.orderHead.ApplicationUser.State;
            ShoppingCartVM.orderHead.PostalCode = ShoppingCartVM.orderHead.ApplicationUser.PostalCode;
            ShoppingCartVM.orderHead.Name = ShoppingCartVM.orderHead.ApplicationUser.Name;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.orderHead.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product");
            ShoppingCartVM.orderHead.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault
                (u => u.Id == claim.Value);
            ShoppingCartVM.orderHead.PaymentsStatus = SD.PaymentStatusPending;
            ShoppingCartVM.orderHead.OrderStatus = SD.StatusPending;
            ShoppingCartVM.orderHead.OrderDate =DateTime.Now;
            ShoppingCartVM.orderHead.ApplicationUserId = claim.Value;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBaseOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.orderHead.OrderTotal += (cart.Price * cart.Count);
            }

            _unitOfWork.OrderHead.Add(ShoppingCartVM.orderHead);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.orderHead.Id,
                    Prices = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetails.Add(orderDetails);
                _unitOfWork.Save();
            }
            _unitOfWork.ShoppingCard.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.Save();
            return RedirectToAction("Index","Home");
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
