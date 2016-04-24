using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using LiveChat.DataAccess.Entities;
using LiveChat.Domain.Services;
using Microsoft.AspNet.Identity;
using System.Linq;

namespace LiveChat.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly IAuthService _authService;
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(IAuthService authService,string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _authService = authService;
            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            ApplicationUser user = await _authService.FindUser(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            if (user.Banned)
            {
                context.SetError("invalid_grant", "User is banned");
                return;
            }

            if (user.Suspended)
            {
                context.SetError("invalid_grant", "User is suspended");
                return;
            }

            ClaimsIdentity oAuth = await _authService
                .CreateIdentity(user, DefaultAuthenticationTypes.ExternalBearer);
            List<Claim> roles = oAuth.Claims
                .Where(c => c.Type == ClaimTypes.Role).ToList();
            AuthenticationProperties properties = CreateProperties(user.UserName, roles);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuth, properties);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName, List<Claim> roles)
        {
            string rolesString = Newtonsoft.Json.JsonConvert.SerializeObject(roles.Select(x => x.Value));
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                {"userName", userName},
                {"roles", rolesString}
            };
            return new AuthenticationProperties(data);
        }
    }
}