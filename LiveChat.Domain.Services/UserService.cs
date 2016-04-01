using LiveChat.DataAcces.Entities.Models;
using LiveChat.DataAccess.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Domain.Services
{
    public interface IUserService
    {
        Task<IEnumerable<AspNetUser>> AllUsers();
        //Task<AspNetUser> GetTruncatedUserByLogin(string name);
        Task<AspNetUser> GetUserByLogin(string userName);
    }

    public class UserService : IUserService
    {
        private readonly ILiveChatContextProvider _liveChatContextProvider;


        public UserService(
            ILiveChatContextProvider eventServeContexProvider)
        {
            _liveChatContextProvider = eventServeContexProvider;
        }

        public async Task<IEnumerable<AspNetUser>> AllUsers()
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                IEnumerable<AspNetUser> users = await context.AspNetUsers.ToListAsync();
                //IEnumerable<TruncatedUser> truncatedUsers = users
                //    .Select(u => _truncatedUserMapper.Map(u));
                return users;
            }
        }

        public async Task<AspNetUser> GetTruncatedUserByLogin(string name)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                AspNetUser user = await context.AspNetUsers
                    .Where(u => u.UserName == name).FirstOrDefaultAsync();
                //TruncatedUser truncatedUser = _truncatedUserMapper.Map(user);
                return user;
            }
        }

        public async Task<AspNetUser> GetUserByLogin(string userName)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                AspNetUser user = await context.AspNetUsers
                    .Where(p => p.UserName == userName)
                    .FirstAsync();
                return user;
            }
        }
    }
}
