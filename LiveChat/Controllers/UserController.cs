using LiveChat.Domain.Models;
using LiveChat.Domain.Services;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace LiveChat.Controllers
{
    [Authorize]
    [RoutePrefix("Api/Users")]
    public class UserController : ApiController
    {
        private IUserService userService;

        public UserController(IUserService _userService)
        {
            userService = _userService;
        }

        [Route("Find")]
        public async Task<IEnumerable<UserReturnModel>> FindUser(string username)
        {
            return await userService.Find(username);
        }
        [Route("Friends")]
        public async Task<IEnumerable<UserReturnModel>> GetFriends()
        {
            var userId = User.Identity.GetUserId();
            return await userService.Friends(userId);
        }
        [Route("RequestFriend")]
        public async Task SendRequest(string id)
        {
            var userId = User.Identity.GetUserId();
            await userService.SendFriendRequest(userId,id);
        }
        [Route("AddFriend")]
        public async Task AddRequest(string id)
        {
            var userId = User.Identity.GetUserId();
            await userService.AcceptFriendRequest(userId, id);
        }
        [Route("DeclineFriend")]
        public async Task RemoveRequest(string id)
        {
            var userId = User.Identity.GetUserId();
            await userService.DeclineFriendRequest(userId, id);
        }
        [Route("GetRequests")]
        public async Task<IEnumerable<UserReturnModel>> GetRequests()
        {
            var userId = User.Identity.GetUserId();
            return await userService.GetRequests(userId);
        }
    }
}
