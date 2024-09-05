using ECommerceApp.Shared;
using System.ComponentModel.DataAnnotations;

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
