using LiveChat.DataAcces.Entities.Models;
using LiveChat.DataAccess.Configuration;
using LiveChat.Domain.Models;
using LiveChat.Domain.Models.Mappers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Domain.Services
{
    public interface IMessengerService
    {
        Task<IEnumerable<MessageReturnModel>> GetLastMessages(string author, string recipient);
        Task<IEnumerable<MessageReturnModel>> GetChattingHistory(string author);
        Task SaveMessage(string author, string recipient, string text);
        Task<string> GetImageUrlByName(string name);
    }

    public class MessengerService : IMessengerService
    {
        private readonly ILiveChatContextProvider _liveChatContextProvider;
        private readonly IMessageReturnModelMapper _messageReturnModelMapper;
        private readonly IUserService _userService;

        public MessengerService(
            ILiveChatContextProvider eventServeContextProvider,
            IMessageReturnModelMapper messageReturnModelMapper,
            IUserService userService)
        {
            _liveChatContextProvider = eventServeContextProvider;
            _messageReturnModelMapper = messageReturnModelMapper;
            _userService = userService;
        }

        public async Task SaveMessage(string author, string recipient, string text)
        {
            if (author == recipient)
            {
                return;
            }
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                Task<AspNetUser> getAuthorTask = _userService.GetUserByLogin(author);
                Task<AspNetUser> getRecipientTask = _userService.GetUserByLogin(recipient);
                await Task.WhenAll(getAuthorTask, getRecipientTask);

                Message message = new Message
                {
                    Author_Id = getAuthorTask.Result.Id,
                    Recipient_Id = getRecipientTask.Result.Id,
                    Time = DateTime.UtcNow.AddHours(3),
                    Text = text,
                    State = EntityState.Added
                };
                context.Messages.Add(message);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MessageReturnModel>> GetLastMessages(string author, string recipient)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var messages = await context.Messages
                    .Where(m => (m.Author.UserName == author && m.Recipient.UserName == recipient) ||
                                (m.Author.UserName == recipient && m.Recipient.UserName == author))
                    .OrderByDescending(m => m.Time)
                    .Take(10)
                    .OrderBy(m => m.Time)
                    .Select(m => new
                    {
                        Message = m,
                        m.Author,
                        m.Recipient
                    }
                    ).ToListAsync();
                var returnModel = messages.Select(s => _messageReturnModelMapper.Map(s.Message));
                return returnModel;
            }
        }

        public async Task<string> GetImageUrlByName(string name)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                string userImageUrl = await context.AspNetUsers
                    .Where(u => u.UserName == name)
                    .Select(p => p.PhoneNumber).FirstAsync();//TODO: remove phone number from here and add image
                return userImageUrl;
            }
        }

        public async Task<IEnumerable<MessageReturnModel>> GetChattingHistory(string author)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                string userId = (await _userService.GetUserByLogin(author)).Id;

                var messages = await context.Messages
                    .Where(m => m.Recipient_Id == userId || m.Author_Id == userId)
                    .Select(m => new
                    {
                        Partner = m.Recipient_Id == userId ? m.Author_Id : m.Recipient_Id,
                        Message = m
                    })
                    .GroupBy(m => m.Partner)
                    .SelectMany(messageGroup => messageGroup.Where(lastMessage =>
                        lastMessage.Message.Time == messageGroup.Max(m => m.Message.Time))
                        .Take(1))
                    .Select(m => new
                    {
                        m.Message,
                        m.Message.Author,
                        m.Message.Recipient
                    })
                    .ToListAsync();

                IEnumerable<MessageReturnModel> returnModel = messages.Select(s =>
                {
                    var mes = _messageReturnModelMapper.Map(s.Message);
                    TruncateMessageText(mes);
                    return mes;
                });

                return returnModel;
            }
        }

        private static void TruncateMessageText(MessageReturnModel mes)
        {
            if (mes.Text.Length >= 1000)
            {
                mes.Text = mes.Text.Substring(0, 1000) + "...";
            }
        }
    }
}
