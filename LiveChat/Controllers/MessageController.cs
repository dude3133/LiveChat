using LiveChat.Domain.Models;
using LiveChat.Domain.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace LiveChat.Controllers
{
    [Authorize]
    [RoutePrefix("Api/Messages")]
    public class MessageController : ApiController
    {
        private IMessengerService messengerService;
        public MessageController(IMessengerService _messengerService)
        {
            messengerService = _messengerService;
        }

        [Route("Send")]
        [HttpPost]
        public async Task<IHttpActionResult> Send(MessageBindingModel model)
        {
            model.AuthorId = User.Identity.GetUserId();
            model.Time = DateTime.UtcNow;
            await messengerService.SaveMessage(model);
            return Ok();
        }

        [Route("Receive")]
        [HttpGet]
        public async Task<IEnumerable<MessageReturnModel>> Receive(string from,int count)
        {
            var x = User.Identity.GetUserName();
            return await messengerService.GetLastMessages(x,from,count);
        }

        [Route("Receive")]
        [HttpGet]
        public async Task<IEnumerable<MessageReturnModel>> Receive(string from,int count, int offset, int lastId)
        {
            var x = User.Identity.GetUserName();
            return await messengerService.GetMessages(x, from, count,offset,lastId);
        }

        [Route("Unread")]
        [HttpGet]
        public async Task<IEnumerable<MessageReturnModel>> Unread(string from)
        {
            var x = User.Identity.GetUserName();
            return await messengerService.GetUnreadMessagesFromDialog(x, from);
        }

        [Route("Read")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string from)
        {
            var x = User.Identity.GetUserName();
            await messengerService.ReadMessagesFromDialog(x, from);
            return Ok();
        }
    }
}
