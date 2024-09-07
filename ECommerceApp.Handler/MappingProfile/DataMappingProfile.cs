using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ECommerceApp.AggregateRoot.Identity;
using ECommerceApp.AggregateRoot.Models;
using ECommerceApp.DTO.ViewModels;

namespace ECommerceApp.Handler.MappingProfile
{
    public class DataMappingProfile : Profile
    {
        public DataMappingProfile()
        {
            // Example: Mapping between Order and EditOrderViewModel
            CreateMap<Order, EditOrderViewModel>()
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.Payment.Status))
                .ReverseMap();

            // Mapping between Product entity and ProductViewModel
            CreateMap<Product, ProductViewModel>().ReverseMap();

            // Mapping between ApplicationUser and RegisterViewModel
            CreateMap<ApplicationUser, RegisterViewModel>().ReverseMap()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)); // Map UserName to Email

            // Mapping between ApplicationUser and LoginViewModel
            CreateMap<ApplicationUser, LoginViewModel>().ReverseMap();

            // You can add more mappings here
        }
    }
}
