using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerceApp.Repository.DBContext;
using ECommerceApp.DTO.ViewModels;
using ECommerceApp.Handler.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceApp.Handler.InterfaceHandler;
using AutoMapper;
using ECommerceApp.Repository.Repository;


namespace ECommerceApp.Handler.ServiceHandler
{
    public class HomeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly HomeRepository _homeRepository; // Inject repository

        // Constructor to inject services
        public HomeService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper, HomeRepository homeRepository)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _homeRepository = homeRepository; // Assign repository
        }

       
        public async Task<List<ProductViewModel>> GetProductsAsync()
        {
            var products = await _homeRepository.GetAllProductsAsync(); // Use repository method
            // Use AutoMapper to map List<Product> to List<ProductViewModel>
            return _mapper.Map<List<ProductViewModel>>(products);
        }

        public async Task<ProductViewModel> GetProductDetailsAsync(Guid id)
        {
            var product = await _homeRepository.GetProductByIdAsync(id); // Use repository method

            if (product == null)
            {
                return null;
            }

            // Use AutoMapper to map Product to ProductViewModel
            return _mapper.Map<ProductViewModel>(product);
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
            var products = _context.Products
                .Where(p => cart.Keys.Contains(p.ProductId))
                .Include(p => p.Category) // Include Category for mapping
                .ToList();

            var productViewModels = _mapper.Map<List<ProductViewModel>>(products);

            // Map quantities from the cart
            foreach (var productViewModel in productViewModels)
            {
                productViewModel.Quantity = cart[productViewModel.ProductId];
            }

            return productViewModels;
        }
    }
}


