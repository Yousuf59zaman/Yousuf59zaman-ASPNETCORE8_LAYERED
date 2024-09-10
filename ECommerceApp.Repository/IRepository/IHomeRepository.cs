using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerceApp.AggregateRoot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceApp.Repository.IRepository
{
    public interface IHomeRepository
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(Guid productId);

        // In IHomeRepository.cs
        Task<List<Product>> GetProductsInCartAsync(Dictionary<Guid, int> cart);

    }
}

