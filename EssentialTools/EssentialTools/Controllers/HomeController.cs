﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EssentialTools.Models;
using Ninject;

namespace EssentialTools.Controllers
{
    public class HomeController : Controller
    {
        private readonly IValueCalculator _calc;

        private Product[] products =
        {
            new Product() {Name = "Kayak", Category = "Watersports", Price = 275M},
            new Product() {Name = "Lifejacket", Category = "Watersports", Price = 48.95M},
            new Product() {Name = "Soccer ball", Category = "Soccer", Price = 19.50M},
            new Product() {Name = "Corner flag", Category = "Soccer", Price = 34.95M}
        };

        public HomeController(IValueCalculator calcParam, IValueCalculator calcParam2)
        {
            _calc = calcParam;
        }
        // GET: Home
        public ActionResult Index()
        {
            ShoppingCart cart = new ShoppingCart(_calc) {Products = products};

            decimal totalValue = cart.CalculateProductTotal();

            return View(totalValue);
        }
    }
}