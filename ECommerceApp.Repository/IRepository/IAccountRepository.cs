using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerceApp.AggregateRoot.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ECommerceApp.Repository.IRepository
{
    public interface IAccountRepository
    {
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<SignInResult> PasswordSignInAsync(string email, string password, bool rememberMe, bool lockoutOnFailure);
        Task SignOutAsync();
    }
}
