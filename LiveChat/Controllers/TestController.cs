using System.Web.Http;

namespace LiveChat.Controllers
{
    public class TestController : ApiController
    {
        //[Authorize]
        //public string Get()
        //{
        //    var x = User.Identity.GetUserId();
        //    return "Your id is "+x;
        //}
        [Authorize]
        public string Get()
        {
            return "Registered";
        }
    }
}
