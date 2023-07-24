using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModel;
using BulkyBooks.Utilitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.ObjectModelRemoting;
using Stripe.Checkout;
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
            if(claimsIdentity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim == null) 
            {
                return RedirectToAction("Error", "Home");
            }

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
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
           

            //stripe setting 
            #region
            var domain = "https://localhost:7276/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> 
                {
                    "Card",

                },
              
                LineItems = new List<SessionLineItemOptions>(),
                   
                Mode = "payment",
                SuccessUrl = domain+$"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.orderHead.Id}",
                CancelUrl = domain+$"Customer/Cart/Index",
            };

            foreach(var item in ShoppingCartVM.ListCart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        },
                    },
                        Quantity = item.Count,
                   
                };
                options.LineItems.Add(sessionLineItem);
            }
            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHead.UpdateStripePaymentID(ShoppingCartVM.orderHead.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            //_unitOfWork.ShoppingCard.RemoveRange(ShoppingCartVM.ListCart);
            // _unitOfWork.Save();
            // return RedirectToAction("Index","Home");
            #endregion
        }

        public IActionResult OrderConfirmation(int id)
        {
            orderHead orderHead = _unitOfWork.OrderHead.GetFirstOrDefault(u => u.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHead.SessionId);
            if (session.PaymentStatus.ToLower()=="paid")
            {
                _unitOfWork.OrderHead.UpdateStatus(id, SD.StatusApprove, SD.PaymentStatusApprove);
                _unitOfWork.Save();
            }
            List<ShoppingCard> shoppingCartVMs = _unitOfWork.ShoppingCard.GetAll(u => u.ApplicationUserId ==
            orderHead.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCard.RemoveRange(shoppingCartVMs);
            _unitOfWork.Save();
            return View(id);
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
