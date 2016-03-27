using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LiveChat.Controllers
{
    public class TestController : ApiController
    {
        [Authorize]
        public string Get()
        {
            var x = User.Identity.GetUserId();
            return "Your id is "+x;
        }

        public int Get(int a)
        {
            return a;
        }
    }
}
