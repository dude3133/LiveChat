using System;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using LiveChat.Providers;
using LiveChat.DataAccess.Configuration;
using Microsoft.AspNet.SignalR;
using LiveChat.App_Start;
using System.Web.Http;
using Unity.WebApi;
using LiveChat.Domain.Services;
using Microsoft.Practices.Unity;

[assembly: OwinStartup(typeof(LiveChat.Startup))]

namespace LiveChat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            UnityConfig.RegisterComponents(app);

            ConfigureAuth(app);

            app.UseWebApi(RegisterHttpConfiguration());
            app.MapSignalR("/signalr", new HubConfiguration
            {
                EnableDetailedErrors = true,
                Resolver = new UnityHubConfig(),
                EnableJavaScriptProxies = false
            });


        }

        public static HttpConfiguration RegisterHttpConfiguration()
        {
            HttpConfiguration config = new HttpConfiguration
            {
                DependencyResolver = new UnityDependencyResolver(UnityConfig.Container)
            };
            WebApiConfig.Register(config);
            return config;
        }

        #region Auth
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(UnityConfig.Container.Resolve<IAuthService>(),PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                // In production mode set AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }
        #endregion
    }
}
