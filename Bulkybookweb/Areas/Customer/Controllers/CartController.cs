﻿using Microsoft.AspNetCore.Mvc;

namespace Bulkybookweb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}