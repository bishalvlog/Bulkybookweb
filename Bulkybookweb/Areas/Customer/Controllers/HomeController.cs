﻿using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Bulkybookweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return View(products);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCard cardobj = new()
            {
                Count = 1,
                ProductId = productId,  
                Product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == productId, includeProperties : "Category,CoverType"),
            };

            return View(cardobj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public  IActionResult Details(ShoppingCard shoppingCard)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCard.ApplicationUserId = claim.Value;

            ShoppingCard carddb = _unitOfWork.ShoppingCard.GetFirstOrDefault(u=>u.ApplicationUserId==claim.Value && u.ProductId==shoppingCard.ProductId);
            if(carddb != null)
            {
                _unitOfWork.ShoppingCard.Add(shoppingCard);
            }
            else
            {
                _unitOfWork.ShoppingCard.IncreamentCount(carddb, shoppingCard.Count);
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));     
            ShoppingCard card = new()
            {
                Count = 1,
            };

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}