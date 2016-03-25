using LiveChat.DataAccess.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using System;

namespace LiveChat.DataAccess.Configuration
{
    public interface IApplicationContextProvider
    {
        IdentityDbContext<ApplicationUser> Context { get; }
        UserManager<ApplicationUser> GetUserManager(IdentityDbContext<ApplicationUser> context);
    }

    public class ApplicationContextProvider : IApplicationContextProvider
    {
        private readonly IIdentityMessageService _service;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public IdentityDbContext<ApplicationUser> Context
        {
            get { return new ApplicationDbContext(); }
        }

        public ApplicationContextProvider(IDataProtectionProvider provider, IIdentityMessageService service)
        {
            _dataProtectionProvider = provider;
            _service = service;
        }

        public UserManager<ApplicationUser> GetUserManager(IdentityDbContext<ApplicationUser> context)
        {
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(context);
            ApplicationUserManager userManager = new ApplicationUserManager(userStore)
            {
                EmailService = _service,
                UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(
                    _dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    //Code for email confirmation and reset password life time
                    TokenLifespan = TimeSpan.FromHours(6)
                }
            };
            return userManager;
        }
    }
}