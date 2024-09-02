using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApp.Models;
namespace ECommerceApp.DTO.ViewModels
{
    public class EditOrderViewModel
    {
        public int OrderID { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        [Required]
        public PaymentStatus PaymentStatus { get; set; }
    }
}
