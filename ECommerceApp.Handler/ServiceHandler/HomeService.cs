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
using ECommerceApp.AggregateRoot.CartAggregateRoot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceApp.Handler.InterfaceHandler;
using AutoMapper;
using ECommerceApp.Repository.Repository;
using ECommerceApp.Repository.IRepository;


namespace ECommerceApp.Handler.ServiceHandler
{
    public class HomeService : IHomeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHomeRepository _homeRepository; // Inject repository
        private readonly CartAggregateRoot _cartAggregateRoot; // Add this

        // Constructor to inject services
        public HomeService(ApplicationDbContext context, IMapper mapper, IHomeRepository homeRepository, CartAggregateRoot cartAggregateRoot)
        {
            _context = context;
            _mapper = mapper;
            _homeRepository = homeRepository; // Use interface instead of concrete class
            _cartAggregateRoot = cartAggregateRoot; // Inject CartAggregateRoot
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

        // Refactored method to use CartAggregateRoot
        public int GetCartCount()
        {
            return _cartAggregateRoot.GetCartCount(); // Use CartAggregateRoot method
        }

        // Refactored method to use CartAggregateRoot
        public Dictionary<Guid, int> GetCart()
        {
            return _cartAggregateRoot.GetCart(); // Use CartAggregateRoot method
        }

        // Refactored method to use CartAggregateRoot
        public void AddToCart(Guid productId, int quantity)
        {
            _cartAggregateRoot.AddToCart(productId, quantity); // Use CartAggregateRoot method
        }

        // Refactored method to use CartAggregateRoot
        public void UpdateCartQuantity(Guid productId, int quantity)
        {
            _cartAggregateRoot.UpdateCartQuantity(productId, quantity); // Use CartAggregateRoot method
        }

        // Refactored method to use CartAggregateRoot
        public void RemoveFromCart(Guid productId)
        {
            _cartAggregateRoot.RemoveFromCart(productId); // Use CartAggregateRoot method
        }

        // Fetch products that are in the cart and map them
        public async Task<List<ProductViewModel>> GetProductsInCartAsync(Dictionary<Guid, int> cart)
        {
            var products = await _context.Products
                .Where(p => cart.Keys.Contains(p.ProductId))
                .Include(p => p.Category) // Include Category for mapping
                .ToListAsync(); // Use async version of ToList()

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

