using ECommerceApp.Repository.DBContext;
using ECommerceApp.DTO.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ECommerceApp.Handler.Utilities; // For serializing and deserializing objects to and from JSON format in session
using ECommerceApp.Handler.InterfaceHandler;
using System;
using System.Threading.Tasks;

namespace ECommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _homeService.GetProductsAsync();
            ViewBag.CartCount = _homeService.GetCartCount();
            return View(products);
        }

        [HttpPost]
        public IActionResult AddToCart(Guid productId, int quantity)
        {
            _homeService.AddToCart(productId, quantity);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Cart()
        {
            var cart = _homeService.GetCart(); // Synchronous call to get the cart
            var productsInCart = await _homeService.GetProductsInCartAsync(cart); // Asynchronous call to get products in the cart
            return View(productsInCart); // Return the view with the list of products in the cart
        }


        public IActionResult RemoveFromCart(Guid id)
        {
            _homeService.RemoveFromCart(id);
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCartQuantity([FromBody] UpdateCartQuantityModel model)
        {
            _homeService.UpdateCartQuantity(model.ProductId, model.Quantity);
            return Ok();
        }
        public class UpdateCartQuantityModel
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
        public async Task<IActionResult> ProductDetails(Guid id)
        {
            var product = await _homeService.GetProductDetailsAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
