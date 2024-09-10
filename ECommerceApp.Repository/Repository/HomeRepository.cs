using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerceApp.AggregateRoot.Models;
using ECommerceApp.Repository.DBContext;
using ECommerceApp.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceApp.Repository.Repository
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _context;

        public HomeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fetch all products including their category
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category) // Include category details
                .ToListAsync();
        }

        // Fetch product details by ID
        public async Task<Product> GetProductByIdAsync(Guid productId)
        {
            return await _context.Products
                .Include(p => p.Category) // Include category details
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<List<Product>> GetProductsInCartAsync(Dictionary<Guid, int> cart)
        {
            return await _context.Products
                .Where(p => cart.Keys.Contains(p.ProductId))
                .Include(p => p.Category) // Include Category for mapping
                .ToListAsync(); // Use async version of ToListAsync
        }

    }
}

