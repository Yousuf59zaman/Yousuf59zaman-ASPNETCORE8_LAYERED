using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceApp.DTO.ViewModels;

namespace ECommerceApp.Handler.InterfaceHandler
{ 
        public interface IHomeService
        {
            Task<List<ProductViewModel>> GetProductsAsync();
            Task<ProductViewModel> GetProductDetailsAsync(Guid id);
            int GetCartCount();
            Dictionary<Guid, int> GetCart();
            void AddToCart(Guid productId, int quantity);
            void UpdateCartQuantity(Guid productId, int quantity);
            void RemoveFromCart(Guid productId);
            List<ProductViewModel> GetProductsInCart(Dictionary<Guid, int> cart);
        }
    

}
