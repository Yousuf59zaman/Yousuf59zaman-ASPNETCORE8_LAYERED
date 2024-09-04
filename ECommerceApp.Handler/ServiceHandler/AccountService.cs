using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApp.Repository.DBContext;
using ECommerceApp.DTO.Identity;
using ECommerceApp.DTO.ViewModels;
using ECommerceApp.Handler.InterfaceHandler;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;


namespace ECommerceApp.Handler.ServiceHandler
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountService> _logger;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            if (model == null)
            {
                _logger.LogWarning("Login attempt with null model.");
                return SignInResult.Failed;
            }

            _logger.LogInformation("Login attempt for email: {Email}", model.Email);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in successfully with email: {Email}", model.Email);
            }
            else
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", model.Email);
            }

            return result;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            if (model == null)
            {
                _logger.LogWarning("Registration attempt with null model.");
                return IdentityResult.Failed();
            }

            _logger.LogInformation("Registering new user with email: {Email}", model.Email);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", model.Email);
                return IdentityResult.Failed(new IdentityError { Description = "A user with this email address already exists." });
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone,
                Age = model.Age,
                Gender = model.Gender,
                Address = model.Address,
                Name = model.Name
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created successfully with email: {Email}", model.Email);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Error occurred while creating user: {ErrorDescription}", error.Description);
                }
            }

            return result;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}

