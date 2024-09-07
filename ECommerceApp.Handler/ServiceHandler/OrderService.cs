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

using AutoMapper;
using ECommerceApp.AggregateRoot.Models;
using ECommerceApp.DTO.ViewModels;
using ECommerceApp.Handler.InterfaceHandler;
using ECommerceApp.Repository.DBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<ProductViewModel>> GetCartProductsAsync(Dictionary<Guid, int> cart)
        {
            var products = await _context.Products
                .Where(p => cart.Keys.Contains(p.ProductId))
                .ToListAsync();

            var productViewModels = _mapper.Map<List<ProductViewModel>>(products);

            // Update quantities from the cart
            productViewModels.ForEach(p => p.Quantity = cart[p.ProductId]);

            return productViewModels;
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
            // Fetch products from the cart
            var products = await _context.Products.Where(p => cart.Keys.Contains(p.ProductId)).ToListAsync();

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

            // Use repository to handle order creation
            return await _orderRepository.PlaceOrderAsync(order, payment);
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
                // Use AutoMapper to update Order entity
                _mapper.Map(model, order);

                // Save updated order
                await _orderRepository.UpdateOrderAsync(order);
            }
            return order;
        }

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



