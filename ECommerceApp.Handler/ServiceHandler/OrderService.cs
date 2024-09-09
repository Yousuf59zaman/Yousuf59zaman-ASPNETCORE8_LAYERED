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
using ECommerceApp.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.Repository.IRepository;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ECommerceApp.Handler.ServiceHandler
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        // Fetches products from the cart and maps them to ProductViewModels with quantities
        public async Task<List<ProductViewModel>> GetCartProductsAsync(Dictionary<Guid, int> cart)
        {
            return await _orderRepository.GetCartProductsAsync(cart);
        }

        // Calculates the total amount for all products in the cart
        public async Task<decimal> GetCartTotalAmountAsync(List<ProductViewModel> products)
        {
            return products.Sum(p => p.ProductUnitPrice * p.Quantity);
        }

        // Retrieves the shipping address for the current user
        public async Task<string> GetShippingAddressAsync(string userId)
        {
            return await _orderRepository.GetShippingAddressAsync(userId);
        }

        // Places an order by creating an Order and a Payment record
        public async Task<Order> PlaceOrderAsync(Dictionary<Guid, int> cart, string userId, string shippingAddress, string paymentMethod)
        {
            return await _orderRepository.PlaceOrderAsync(cart, userId, shippingAddress, paymentMethod);
        }

        // Fetches a specific order by ID
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        // Fetches the order history of a specific user
        public async Task<List<Order>> GetUserOrderHistoryAsync(string userId)
        {
            return await _orderRepository.GetUserOrderHistoryAsync(userId);
        }

        // Updates an existing order based on the EditOrderViewModel
        public async Task<Order> UpdateOrderAsync(EditOrderViewModel model)
        {
            var order = await _orderRepository.GetOrderByIdAsync(model.OrderID);
            if (order != null)
            {
                // Use AutoMapper to update Order entity from EditOrderViewModel
                _mapper.Map(model, order);

                // Save the updated order
                await _orderRepository.UpdateOrderAsync(order);
            }
            return order;
        }

        // Deletes an order based on the order ID
        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order != null)
            {
                await _orderRepository.DeleteOrderAsync(order);
            }
        }
    }
}




