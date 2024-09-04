using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApp.DTO.ViewModels;
using ECommerceApp.DTO.Models;


namespace ECommerceApp.Handler.InterfaceHandler
{
    public interface IOrderService
    {
        Task<List<ProductViewModel>> GetCartProductsAsync(Dictionary<Guid, int> cart);
        Task<decimal> GetCartTotalAmountAsync(List<ProductViewModel> products);
        Task<string> GetShippingAddressAsync(string userId);
        Task<Order> PlaceOrderAsync(Dictionary<Guid, int> cart, string userId, string shippingAddress, string paymentMethod);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetUserOrderHistoryAsync(string userId);
        Task<Order> UpdateOrderAsync(EditOrderViewModel model);
        Task DeleteOrderAsync(int orderId);
    }
}

