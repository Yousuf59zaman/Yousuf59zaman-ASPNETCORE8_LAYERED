using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ECommerceApp.AggregateRoot.Models
{
    public class Category
    {
        public Guid CategoryId { get; set; }  // Primary Key
        public string CategoryType { get; set; }
        public string CategoryName { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; }  // One-to-many relationship with Products
    }
}
