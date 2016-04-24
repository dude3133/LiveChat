using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Owin;
using System.Security.Claims;

namespace LiveChat.Hubs
{
    public sealed class QueryStringBearerAuthorizeAttribute : AuthorizeAttribute
    {
        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {

            var token = request.QueryString.Get("token");//const
            var authenticationTicket = Startup.OAuthOptions.AccessTokenFormat.Unprotect(token);

            if (authenticationTicket == null || authenticationTicket.Identity == null ||
                !authenticationTicket.Identity.IsAuthenticated)
            {
                return false;
            }
            request.GetHttpContext().User = new ClaimsPrincipal(authenticationTicket.Identity);
            return true;

        }

        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            var connectionId = hubIncomingInvokerContext.Hub.Context.ConnectionId;
            var environment = hubIncomingInvokerContext.Hub.Context.Request.Environment;
            var principal = hubIncomingInvokerContext.Hub.Context.Request.GetHttpContext().User;

            if (principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
            {
                hubIncomingInvokerContext.Hub.Context = new HubCallerContext(new ServerRequest(environment), connectionId);
                return true;
            }
            return false;
        }
    }
}