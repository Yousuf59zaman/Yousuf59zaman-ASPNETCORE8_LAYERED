using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerceApp.AggregateRoot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ECommerceApp.AggregateRoot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.DTO.ViewModels;

namespace ECommerceApp.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetUserOrderHistoryAsync(string userId);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);

        // New methods for handling cart and orders
        Task<List<ProductViewModel>> GetCartProductsAsync(Dictionary<Guid, int> cart);
        Task<Order> PlaceOrderAsync(Dictionary<Guid, int> cart, string userId, string shippingAddress, string paymentMethod);
        Task<string> GetShippingAddressAsync(string userId);
    }
}


