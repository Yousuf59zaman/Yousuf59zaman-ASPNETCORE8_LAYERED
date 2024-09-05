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
namespace ECommerceApp.Handler.ServiceHandler
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var orderDetails = new List<OrderDetails>();
            decimal totalAmount = 0;

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

            var order = new Order
            {
                CustomerID = userId,
                OrderStatus = "Pending",
                TotalAmount = totalAmount,
                ShippingAddress = shippingAddress,
                OrderDetails = orderDetails
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var paymentStatus = paymentMethod == "Credit Card" ? PaymentStatus.Successful : PaymentStatus.Pending;

            var payment = new Payment
            {
                OrderID = order.OrderID,
                PaymentAmount = totalAmount,
                Method = paymentMethod,
                Status = paymentStatus
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            order.PaymentID = payment.PaymentID;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }

        public async Task<List<Order>> GetUserOrderHistoryAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.CustomerID == userId)
                .ToListAsync();
        }

        public async Task<Order> UpdateOrderAsync(EditOrderViewModel model)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.OrderID == model.OrderID);

            if (order != null)
            {
                order.OrderStatus = model.OrderStatus;
                if (order.Payment != null)
                {
                    order.Payment.Status = model.PaymentStatus;
                }

                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }

            return order;
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order != null)
            {
                if (order.Payment != null)
                {
                    _context.Payments.Remove(order.Payment);
                }

                _context.OrderDetails.RemoveRange(order.OrderDetails);
                _context.Orders.Remove(order);

                await _context.SaveChangesAsync();
            }
        }
    }
}

