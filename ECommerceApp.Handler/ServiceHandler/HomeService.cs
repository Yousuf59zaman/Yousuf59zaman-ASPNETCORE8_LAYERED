using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerceApp.DTO.DBContext;
using ECommerceApp.DTO.ViewModels;
using ECommerceApp.Handler.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceApp.Handler.InterfaceHandler;

namespace ECommerceApp.Handler.Services
{
    public class HomeService : IHomeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<ProductViewModel>> GetProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)  // Include Category to get CategoryName
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDesc = p.ProductDesc,
                    ProductUnitPrice = p.ProductUnitPrice,
                    ProductImage = $"/images/{p.ProductImage}",  // Assume the image file name is stored in the ProductImage field
                    CategoryName = p.Category.CategoryName
                })
                .ToListAsync();
        }

        public async Task<ProductViewModel> GetProductDetailsAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductId == id)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDesc = p.ProductDesc,
                    ProductUnitPrice = p.ProductUnitPrice,
                    ProductImage = $"/images/{p.ProductImage}",
                    CategoryName = p.Category.CategoryName
                })
                .FirstOrDefaultAsync();
        }

        public int GetCartCount()
        {
            var cart = _httpContextAccessor.HttpContext.Session.Get<Dictionary<Guid, int>>("Cart");
            return cart?.Count ?? 0;
        }

        public Dictionary<Guid, int> GetCart()
        {
            return _httpContextAccessor.HttpContext.Session.Get<Dictionary<Guid, int>>("Cart") ?? new Dictionary<Guid, int>();
        }

        public void AddToCart(Guid productId, int quantity)
        {
            var cart = GetCart();

            if (cart.ContainsKey(productId))
            {
                cart[productId] += quantity;  // Update quantity if the product is already in the cart
            }
            else
            {
                cart.Add(productId, quantity);  // Add new product with the specified quantity
            }

            _httpContextAccessor.HttpContext.Session.Set("Cart", cart);
        }

        public void UpdateCartQuantity(Guid productId, int quantity)
        {
            var cart = GetCart();

            if (cart.ContainsKey(productId))
            {
                if (quantity > 0)
                {
                    cart[productId] = quantity;  // Update quantity in the session
                }
                else
                {
                    cart.Remove(productId);  // If the quantity is zero or less, remove the item
                }
            }

            _httpContextAccessor.HttpContext.Session.Set("Cart", cart);
        }

        public void RemoveFromCart(Guid productId)
        {
            var cart = GetCart();

            if (cart.ContainsKey(productId))
            {
                cart.Remove(productId);  // Remove the product from the cart
            }

            _httpContextAccessor.HttpContext.Session.Set("Cart", cart);
        }

        public List<ProductViewModel> GetProductsInCart(Dictionary<Guid, int> cart)
        {
            return _context.Products
                .Where(p => cart.Keys.Contains(p.ProductId))
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductDesc = p.ProductDesc,
                    ProductUnitPrice = p.ProductUnitPrice,
                    ProductImage = $"/images/{p.ProductImage}",
                    CategoryName = p.Category.CategoryName,
                    Quantity = cart[p.ProductId]  // Get the quantity from the session cart
                })
                .ToList();
        }
    }
}

