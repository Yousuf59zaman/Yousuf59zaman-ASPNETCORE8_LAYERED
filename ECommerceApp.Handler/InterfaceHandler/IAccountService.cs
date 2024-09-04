using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerceApp.DTO.ViewModels;
using ECommerceApp.DTO.Identity;
using Microsoft.AspNetCore.Identity;


namespace ECommerceApp.Handler.InterfaceHandler
{
    public interface IAccountService
    {
        Task<SignInResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<ApplicationUser> FindByEmailAsync(string email);
    }
}

