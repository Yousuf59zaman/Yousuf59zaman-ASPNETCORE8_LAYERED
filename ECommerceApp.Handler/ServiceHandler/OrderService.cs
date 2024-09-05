using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerceApp.AggregateRoot.Models;
using ECommerceApp.DTO.ViewModels;
using ECommerceApp.Handler.InterfaceHandler;
using ECommerceApp.Repository.DBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ECommerceApp.AggregateRoot.Identity;
using ECommerceApp.Shared;
using ECommerceApp.AggregateRoot.Models;
using ECommerceApp.DTO.ViewModels;
using ECommerceApp.Handler.InterfaceHandler;
using ECommerceApp.Repository.Repository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.Repository.IRepository;

namespace ECommerceApp.Handler.ServiceHandler
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public OrderService(IOrderRepository orderRepository, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _context = context;
        }

        public async Task<List<ProductViewModel>> GetCartProductsAsync(Dictionary<Guid, int> cart)
        {
            return await _context.Products
                .Where(p => cart.Keys.Contains(p.ProductId))
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDesc = p.ProductDesc,
                    ProductUnitPrice = p.ProductUnitPrice,
                    ProductImage = $"/images/{p.ProductImage}",
                    CategoryName = p.Category.CategoryName,
                    Quantity = cart[p.ProductId]
                })
                .ToListAsync();
        }

        public async Task<decimal> GetCartTotalAmountAsync(List<ProductViewModel> products)
        {
            return products.Sum(p => p.ProductUnitPrice * p.Quantity);
        }

        public async Task<string> GetShippingAddressAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.Address;
        }

        public async Task<Order> PlaceOrderAsync(Dictionary<Guid, int> cart, string userId, string shippingAddress, string paymentMethod)
        {
            // Initialize the totalAmount and orderDetails list
            decimal totalAmount = 0;
            var orderDetails = new List<OrderDetails>();

            // Iterate through the cart and calculate total amount and order details
            foreach (var item in cart)
            {
                var product = await _context.Products.FindAsync(item.Key);
                if (product != null)
                {
                    totalAmount += product.ProductUnitPrice * item.Value;

                    orderDetails.Add(new OrderDetails
                    {
                        ProductID = product.ProductId,
                        Quantity = item.Value,
                        UnitPrice = product.ProductUnitPrice,
                        Total = product.ProductUnitPrice * item.Value
                    });
                }
            }

            // Create a new Order object with the calculated totalAmount and orderDetails
            var order = new Order
            {
                CustomerID = userId,
                OrderStatus = "Pending",
                ShippingAddress = shippingAddress,
                TotalAmount = totalAmount,
                OrderDetails = orderDetails
            };

            // Use the repository to add the order to the database
            await _orderRepository.AddOrderAsync(order);

            // Continue with payment logic and saving the order
            // (This can still remain in the service layer, as planned.)

            return order;
        }


        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<List<Order>> GetUserOrderHistoryAsync(string userId)
        {
            return await _orderRepository.GetUserOrderHistoryAsync(userId);
        }

        public async Task<Order> UpdateOrderAsync(EditOrderViewModel model)
        {
            var order = await _orderRepository.GetOrderByIdAsync(model.OrderID);
            if (order != null)
            {
                order.OrderStatus = model.OrderStatus;
                if (order.Payment != null)
                {
                    order.Payment.Status = model.PaymentStatus;
                }

                await _orderRepository.UpdateOrderAsync(order); // Use repository for updates
            }

            return order;
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order != null)
            {
                await _orderRepository.DeleteOrderAsync(order); // Use repository for deletion
            }
        }

       
    }
}


