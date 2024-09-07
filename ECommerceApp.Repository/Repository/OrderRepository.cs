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
                .Include(o => o.Payment)  // Include Payment details
                .Include(o => o.OrderDetails)  // Include OrderDetails if necessary
                .ThenInclude(od => od.Product)  // Optionally include Product details if needed
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }


        public async Task<List<Order>> GetUserOrderHistoryAsync(string userId)
        {
            return await _context.Orders
     .Include(o => o.Payment)  // This ensures the Payment data is loaded
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
            // Ensure the Order and Payment are being tracked and updated
            _context.Orders.Update(order);  // This tracks changes to the Order entity
            if (order.Payment != null)
            {
                _context.Payments.Update(order.Payment);  // Explicitly update Payment entity if it exists
            }

            await _context.SaveChangesAsync();  // Persist the changes to the database
        }




        public async Task DeleteOrderAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}

