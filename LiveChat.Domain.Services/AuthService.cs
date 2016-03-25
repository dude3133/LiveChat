using LiveChat.Domain.Models;
using System;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using LiveChat.DataAccess.Entities;
using LiveChat.DataAccess.Configuration;

namespace LiveChat.Domain.Services
{
    public interface IAuthService
    {
        Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo);
        Task<IdentityResult> CreateAsync(ApplicationUser user);
        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);

        Task<IdentityResult> RegisterUser(RegisterUserBindingModel registerUserModel);
        Task<ApplicationUser> FindUser(string userName, string password);
        Task<ClaimsIdentity> CreateIdentity(ApplicationUser user, string authenticationType);
    }

    public class AuthService : IAuthService
    {
        private readonly IApplicationContextProvider _identityProvider;

        public AuthService(IApplicationContextProvider identityProvider)
        {
            _identityProvider = identityProvider;
        }

        public async Task<ApplicationUser> FindAsync(UserLoginInfo loginInfo)
        {
            using (IdentityDbContext<ApplicationUser> context = _identityProvider.Context)
            {
                UserManager<ApplicationUser> userManager =
                    _identityProvider.GetUserManager(context);
                ApplicationUser user = await userManager.FindAsync(loginInfo);
                return user;
            }
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            using (IdentityDbContext<ApplicationUser> context = _identityProvider.Context)
            {
                UserManager<ApplicationUser> userManager =
                    _identityProvider.GetUserManager(context);
                var result = await userManager.CreateAsync(user);
                return result;
            }
        }

        
        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            using (IdentityDbContext<ApplicationUser> context = _identityProvider.Context)
            {
                UserManager<ApplicationUser> userManager =
                    _identityProvider.GetUserManager(context);
                var result = await userManager.AddLoginAsync(userId, login);
                return result;
            }
        }

        public async Task<IdentityResult> RegisterUser(RegisterUserBindingModel registerUserModel)
        {
            ApplicationUser user = new ApplicationUser
            {
                UserName = registerUserModel.Username,
                CreationTime = DateTime.UtcNow,
                Email = registerUserModel.Email,
                EmailConfirmed = false,
                Suspended = false
            };

            using (IdentityDbContext<ApplicationUser> context = _identityProvider.Context)
            {
                UserManager<ApplicationUser> userManager = 
                    _identityProvider.GetUserManager(context);
                
                IdentityResult result = await userManager.
                    CreateAsync(user, registerUserModel.Password);
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "User");
                }
                return result;
            }
        }

        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            using (IdentityDbContext<ApplicationUser> context = _identityProvider.Context)
            {
                UserManager<ApplicationUser> userManager = 
                    _identityProvider.GetUserManager(context);
                
                ApplicationUser user = await userManager.FindAsync(userName, password);
                return user;
            }
        }

        public async Task<ClaimsIdentity> CreateIdentity(
            ApplicationUser user, string authenticationType)
        {
            using (IdentityDbContext<ApplicationUser> context = _identityProvider.Context)
            {
                var userManager = _identityProvider.GetUserManager(context);
                return await userManager.CreateIdentityAsync(user, authenticationType);
            }
        }


    }
}
