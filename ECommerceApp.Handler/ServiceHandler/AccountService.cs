using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerceApp.Repository.DBContext;
using ECommerceApp.AggregateRoot.Identity;
using ECommerceApp.DTO.ViewModels;
using ECommerceApp.Handler.InterfaceHandler;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ECommerceApp.Repository.IRepository;
using System.Threading.Tasks;
using AutoMapper;
using System.Threading.Tasks;

namespace ECommerceApp.Handler.ServiceHandler
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper; // Inject AutoMapper

        public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            if (model == null)
            {
                _logger.LogWarning("Login attempt with null model.");
                return SignInResult.Failed;
            }

            _logger.LogInformation("Login attempt for email: {Email}", model.Email);

            var result = await _accountRepository.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

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
            await _accountRepository.SignOutAsync();
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

            var existingUser = await _accountRepository.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", model.Email);
                return IdentityResult.Failed(new IdentityError { Description = "A user with this email address already exists." });
            }

            // Use AutoMapper to map RegisterViewModel to ApplicationUser
            var user = _mapper.Map<ApplicationUser>(model);

            var result = await _accountRepository.CreateUserAsync(user, model.Password);

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
            return await _accountRepository.FindByEmailAsync(email);
        }
    }
}


