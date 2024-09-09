using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Repository.DBContext;
using ECommerceApp.DTO.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using ECommerceApp.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceApp.Handler.Utilities;
using Microsoft.AspNetCore.Identity;
using ECommerceApp.AggregateRoot.Identity;
using ECommerceApp.AggregateRoot.Models;
using ECommerceApp.Shared;
using ECommerceApp.Handler.InterfaceHandler;

namespace ECommerceApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = HttpContext.Session.Get<Dictionary<Guid, int>>("Cart") ?? new Dictionary<Guid, int>();

            var productsInCart = await _orderService.GetCartProductsAsync(cart);
            ViewBag.TotalAmount = await _orderService.GetCartTotalAmountAsync(productsInCart);
            ViewBag.ShippingAddress = user.Address;

            return View(productsInCart);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string ShippingAddress, string PaymentMethod)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = HttpContext.Session.Get<Dictionary<Guid, int>>("Cart") ?? new Dictionary<Guid, int>();

            if (!cart.Any())
            {
                return RedirectToAction("Cart", "Home");
            }

            var order = await _orderService.PlaceOrderAsync(cart, user.Id, ShippingAddress, PaymentMethod);
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderID });
        }


        [HttpGet("OrderConfirmation/{orderId}")]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();

            return View(order);
        }

        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _orderService.GetUserOrderHistoryAsync(user.Id);

            return View(orders);
        }

        public async Task<IActionResult> EditOrder(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound();

            var viewModel = new EditOrderViewModel
            {
                OrderID = order.OrderID,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.Payment?.Status ?? PaymentStatus.Pending
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateOrder(EditOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Debugging: Check model values
                Console.WriteLine($"Order Status: {model.OrderStatus}, Payment Status: {model.PaymentStatus}");

                await _orderService.UpdateOrderAsync(model);
                return RedirectToAction("OrderHistory");
            }

            return View("EditOrder", model);
        }



        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            await _orderService.DeleteOrderAsync(orderId);
            return RedirectToAction("OrderHistory");
        }
    }
}
