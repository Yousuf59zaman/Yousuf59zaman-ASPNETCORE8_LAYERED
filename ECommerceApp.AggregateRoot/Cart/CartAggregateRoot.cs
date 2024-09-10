using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApp.Handler.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;


namespace ECommerceApp.AggregateRoot.CartAggregateRoot
{
        public class CartAggregateRoot
        {
            private readonly IHttpContextAccessor _httpContextAccessor;

            public CartAggregateRoot(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
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
                    cart[productId] += quantity;
                }
                else
                {
                    cart.Add(productId, quantity);
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
                        cart[productId] = quantity;
                    }
                    else
                    {
                        cart.Remove(productId);
                    }
                }

                _httpContextAccessor.HttpContext.Session.Set("Cart", cart);
            }

            public void RemoveFromCart(Guid productId)
            {
                var cart = GetCart();

                if (cart.ContainsKey(productId))
                {
                    cart.Remove(productId);
                }

                _httpContextAccessor.HttpContext.Session.Set("Cart", cart);
            }
        }
    }


