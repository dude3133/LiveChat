using LiveChat.DataAccess.Configuration;
using LiveChat.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Practices.Unity;
using Owin;

namespace LiveChat
{
    public static class UnityConfig
    {
        public static UnityContainer Container { get; set; }

        public static void RegisterComponents(IAppBuilder app)
        {
            Container = new UnityContainer();
            SetDependencies(Container, app);
        }

        private static void SetDependencies(UnityContainer container, IAppBuilder app)
        {
            //Instances
            container.RegisterInstance<IDataProtectionProvider>(app.GetDataProtectionProvider());

            // DataAccess
            container.RegisterType<ILiveChatContext, LiveChatContext>();


            // Identity
            container.RegisterType<IdentityUser, ApplicationUser>();
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            //container.RegisterType<IdentityDbContext<ApplicationUser>, LiveChatIdentityContext>();
            //container.RegisterType<ILiveChatIdentityProvider, EventServeIdentityProvider>();
            //container.RegisterType<IExternalAccessTokenProvider, ExternalAccessTokenProvider>();


        }
    }
}