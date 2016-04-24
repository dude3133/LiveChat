using LiveChat.DataAccess.Configuration;
using LiveChat.DataAccess.Entities.Models;
using LiveChat.Domain.Models;
using LiveChat.Domain.Models.Mappers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace LiveChat.Domain.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserReturnModel>> AllUsers();
        Task<UserReturnModel> GetTruncatedUserByLogin(string name);
        Task<UserReturnModel> GetUserByLogin(string userName);
        Task<IEnumerable<UserReturnModel>> Find(string username);
        Task<IEnumerable<UserReturnModel>> Friends(string id);
        Task SendFriendRequest(string userId, string id);
        Task AcceptFriendRequest(string userId, string id);
        Task DeclineFriendRequest(string userId, string id);
        Task<IEnumerable<UserReturnModel>> GetRequests(string userId);
    }

    public class UserService : IUserService
    {
        private readonly ILiveChatContextProvider _liveChatContextProvider;
        private readonly IUserReturnModelMapper _truncatedUserMapper;

        public UserService(IUserReturnModelMapper userReturnModel,
            ILiveChatContextProvider liveChatContexProvider)
        {
            _liveChatContextProvider = liveChatContexProvider;
            _truncatedUserMapper = userReturnModel;
        }

        public async Task<IEnumerable<UserReturnModel>> AllUsers()
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                IEnumerable<AspNetUser> users = await context.AspNetUsers.ToListAsync();
                IEnumerable<UserReturnModel> truncatedUsers = users
                    .Select(u => _truncatedUserMapper.Map(u));
                return truncatedUsers;
            }
        }

        public async Task<IEnumerable<UserReturnModel>> Find(string username)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var users = await context.AspNetUsers
                    .Where(u => u.UserName.Contains(username)).ToListAsync();
                return users.Select(x => _truncatedUserMapper.Map(x));
            }
        }

        public async Task<IEnumerable<UserReturnModel>> Friends(string id)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var friends = await context.Friends
                    .Where(u => u.Receiver_Id==id || u.Requestor_Id==id).ToListAsync();
                var friendsIds = friends.Select(x => x.Receiver_Id == id ? x.Requestor_Id : x.Receiver_Id).ToList();
                var users = await context.AspNetUsers
                    .Where(u => friendsIds.Contains(u.Id)).ToListAsync();

                return users.Select(x => _truncatedUserMapper.Map(x));
            }
        }

        public async Task<UserReturnModel> GetTruncatedUserByLogin(string name)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                AspNetUser user = await context.AspNetUsers
                    .Where(u => u.UserName == name).FirstOrDefaultAsync();
                UserReturnModel truncatedUser = _truncatedUserMapper.Map(user);
                return truncatedUser;
            }
        }

        public async Task<UserReturnModel> GetUserByLogin(string userName)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                AspNetUser user = await context.AspNetUsers
                    .Where(p => p.UserName == userName)
                    .FirstAsync();
                return _truncatedUserMapper.Map(user);
            }
        }

        public async Task SendFriendRequest(string userId, string id)
        {
            if (isFriends(userId, id) || isFriendRequestSended(userId, id)) return;
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var request = new FriendRequest
                {
                    Requestor_Id = userId,
                    Receiver_Id = id,
                    Date = DateTime.Now,
                    State = EntityState.Added
                };
                context.FriendRequests.Add(request);
                await context.SaveChangesAsync();
            }
        }

        public async Task AcceptFriendRequest(string userId, string id)
        {
            if (!isFriendRequestSended(id, userId)) return;
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var request = await context.FriendRequests.Where(p => 
                p.Receiver_Id == userId && p.Requestor_Id == id).FirstOrDefaultAsync();
                request.State = EntityState.Deleted;

                var friend = new Friend { Requestor_Id = id, Receiver_Id = userId, Date = DateTime.Now, State = EntityState.Added };
                context.Friends.Add(friend);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeclineFriendRequest(string userId, string id)
        {
            if (!isFriendRequestSended(id, userId)) return;
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var request = await context.FriendRequests.Where(p =>
                p.Receiver_Id == userId && p.Requestor_Id == id).FirstOrDefaultAsync();
                request.State = EntityState.Deleted;
                await context.SaveChangesAsync();
            }
        }

        private bool isFriends(string userId, string id)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                return (context.Friends.Where(p => p.Requestor_Id == userId
                    && p.Receiver_Id == id || p.Requestor_Id == id
                    && p.Receiver_Id == userId).Any());
            } 
        }
        private bool isFriendRequestSended(string userId, string id)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                return (context.FriendRequests
                    .Where(p => p.Requestor_Id == userId
                    && p.Receiver_Id == id).Any());
            }
        }

        public async Task<IEnumerable<UserReturnModel>> GetRequests(string userId)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var requestorsIds = await context.FriendRequests.Where(p =>
                p.Receiver_Id == userId).Select(x => x.Requestor_Id).ToListAsync();
                var users = await context.AspNetUsers
                  .Where(u => requestorsIds.Contains(u.Id)).ToListAsync();
                return users.Select(x => _truncatedUserMapper.Map(x));
            }
        }
    }
}
