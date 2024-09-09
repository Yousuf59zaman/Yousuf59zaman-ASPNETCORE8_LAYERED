using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApp.AggregateRoot.Models;
using ECommerceApp.Repository.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.Repository.IRepository;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.DTO.ViewModels;
using ECommerceApp.Shared;


namespace ECommerceApp.Repository.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Payment)
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

        public async Task AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            if (order.Payment != null)
            {
                _context.Payments.Update(order.Payment);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        // New method to add a payment record
        public async Task AddPaymentAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductViewModel>> GetCartProductsAsync(Dictionary<Guid, int> cart)
        {
            var products = await _context.Products
                .Where(p => cart.Keys.Contains(p.ProductId))
                .ToListAsync();

            return products.Select(p => new ProductViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductUnitPrice = p.ProductUnitPrice,
                Quantity = cart[p.ProductId]
            }).ToList();
        }

        public async Task<Order> PlaceOrderAsync(Dictionary<Guid, int> cart, string userId, string shippingAddress, string paymentMethod)
        {
            // Fetch products from the cart
            var products = await _context.Products
                .Where(p => cart.Keys.Contains(p.ProductId))
                .ToListAsync();

            // Calculate totalAmount and map to OrderDetails
            var orderDetails = products.Select(p => new OrderDetails
            {
                ProductID = p.ProductId,
                Quantity = cart[p.ProductId],
                UnitPrice = p.ProductUnitPrice,
                Total = p.ProductUnitPrice * cart[p.ProductId]
            }).ToList();

            // Calculate total amount
            var totalAmount = orderDetails.Sum(od => od.Total);

            // Create new Order entity
            var order = new Order
            {
                CustomerID = userId,
                OrderStatus = "Pending",
                ShippingAddress = shippingAddress,
                TotalAmount = totalAmount,
                OrderDetails = orderDetails
            };

            // Set payment status based on payment method
            var paymentStatus = paymentMethod == "Credit Card" ? PaymentStatus.Successful : PaymentStatus.Pending;

            // Create Payment entity
            var payment = new Payment
            {
                PaymentAmount = totalAmount,
                Method = paymentMethod,
                Status = paymentStatus,
                PaymentDate = DateTime.Now
            };

            // Use transaction to place order and add payment
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Add order and payment
                    await AddOrderAsync(order);
                    payment.OrderID = order.OrderID;
                    await AddPaymentAsync(payment);

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return order;
        }

        public async Task<string> GetShippingAddressAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.Address;
        }


    }
}


