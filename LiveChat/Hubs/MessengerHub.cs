using LiveChat.Domain.Models;
using LiveChat.Domain.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LiveChat.Hubs
{
    [QueryStringBearerAuthorize]
    [HubName("Messenger")]
    public class MessengerHub : Hub
    {
        private readonly IMessengerService _messengerService;
        private readonly IUserService _userService;

        private readonly static ConcurrentDictionary<string, UserMessengerData> _onlineUsersRepo
                    = new ConcurrentDictionary<string, UserMessengerData>();

        public MessengerHub(
            IMessengerService messengerService,
            IUserService userService)
        {
            _messengerService = messengerService;
            _userService = userService;
        }

        public override Task OnConnected()
        {
            UserMessengerData user = new UserMessengerData
            {
                ConnectedTime = DateTime.UtcNow,
                Name = GetCurrentUserLoginName()
            };
            AddOrUpdateUserInDictionary(user);

            return Clients.All.onlineUserCount(_onlineUsersRepo.Count, _onlineUsersRepo);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            DeleteOrUpdateCurrentUserInDictionary();
            return Clients.All.onlineUserCount(_onlineUsersRepo.Count, _onlineUsersRepo);
        }

        public Task GetOnlineUsers()
        {
            return Clients.Caller.onlineUserCount(_onlineUsersRepo.Count, _onlineUsersRepo);
        }

        public async Task SendMessage(string recipient, string message)
        {
            if (recipient != null)
            {
                await _messengerService.SaveMessage(GetCurrentUserLoginName(), recipient, message);

                if (_onlineUsersRepo.ContainsKey(recipient))
                {
                    SendMessageToRecipient(recipient, message);
                }

                Clients.Caller
                     .sendSelfMessage(GetCurrentUserLoginName(), recipient, DateTime.UtcNow, message);
            }
        }

        public async Task GetChattingHistory()
        {
            IEnumerable<MessageReturnModel> lastTenMsg = await _messengerService.GetChattingHistory(GetCurrentUserLoginName());
            Clients.Caller.takeChattingHistory(lastTenMsg.ToList());
        }

        public async Task GetLastMessages(string recipient)
        {
            IEnumerable<MessageReturnModel> lastTenMsg = await _messengerService
                .GetLastMessages(GetCurrentUserLoginName(), recipient);

            Clients.Caller.takeLastMessages(lastTenMsg.ToList());
        }

        public async Task GetAvatarsToPrivateChat(string recipient)
        {
            Task<string> t1 = _messengerService.GetImageUrlByName(GetCurrentUserLoginName());
            Task<string> t2 = _messengerService.GetImageUrlByName(recipient);
            await Task.WhenAll(t1, t2);
            Clients.Caller.takeAvatarsToPrivateChat(t1.Result, t2.Result);
        }

        private void SendMessageToRecipient(string recipient, string message)
        {
            if (_onlineUsersRepo[recipient].ConnectionsIdList.Count > 1)
            {
                foreach (string conId in _onlineUsersRepo[recipient].ConnectionsIdList)
                {
                    Clients.Client(conId)
                        .sendMessage(GetCurrentUserLoginName(), recipient, DateTime.UtcNow, message);
                }
            }
            else
            {
                Clients.Client(_onlineUsersRepo[recipient].ConnectionsIdList[0])
                    .sendMessage(GetCurrentUserLoginName(), recipient, DateTime.UtcNow, message);
            }
        }

        private void AddConnectionIdToUser()
        {
            if (_onlineUsersRepo.ContainsKey(GetCurrentUserLoginName()))
            {
                _onlineUsersRepo[GetCurrentUserLoginName()].ConnectionsIdList.Add(Context.ConnectionId);
            }
        }

        private string GetCurrentUserLoginName()
        {
            return Context.Request.QueryString.Get("loginName");
        }

        private void AddOrUpdateUserInDictionary(UserMessengerData user)
        {
            if (_onlineUsersRepo.ContainsKey(GetCurrentUserLoginName()))
            {
                AddConnectionIdToUser();
            }
            else
            {
                _onlineUsersRepo.AddOrUpdate(GetCurrentUserLoginName(), user, (key, oldValue) => user);
                AddConnectionIdToUser();
            }
        }

        private void DeleteOrUpdateCurrentUserInDictionary()
        {
            if (_onlineUsersRepo[GetCurrentUserLoginName()].ConnectionsIdList.Count > 1)
            {
                DeleteCurrentConnectionIdInList();
            }
            else
            {
                UserMessengerData removedUser;
                _onlineUsersRepo.TryRemove(GetCurrentUserLoginName(), out removedUser);
            }
        }

        private void DeleteCurrentConnectionIdInList()
        {
            int indexOfCurrentConnectionId = _onlineUsersRepo[GetCurrentUserLoginName()]
                .ConnectionsIdList.FindIndex(s => s.Contains(Context.ConnectionId));

            _onlineUsersRepo[GetCurrentUserLoginName()].ConnectionsIdList.RemoveAt(indexOfCurrentConnectionId);
        }
    }
}