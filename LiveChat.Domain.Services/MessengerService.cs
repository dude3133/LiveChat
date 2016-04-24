using LiveChat.DataAccess.Entities.Models;
using LiveChat.DataAccess.Configuration;
using LiveChat.Domain.Models;
using LiveChat.Domain.Models.Mappers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace LiveChat.Domain.Services
{
    public interface IMessengerService
    {
        Task<IEnumerable<MessageReturnModel>> GetLastMessages(string author, string recipient,int count = 10);
        /// <summary>
        /// Gives messages from some dialog with offset and where Id of oldest message is bigger than lastId
        /// </summary>
        /// <param name="author">Username of author</param>
        /// <param name="recipient">Username of recipient</param>
        /// <param name="count">Count of messages</param>
        /// <param name="offset">Offset</param>
        /// <param name="lastId">Id of oldest message that should be taken</param>
        /// <returns></returns>
        Task<IEnumerable<MessageReturnModel>> GetMessages(string author, string recipient, int count = 1, int offset = 10, int lastId = 1);
        Task<IEnumerable<MessageReturnModel>> GetChattingHistory(string author);
        Task SaveMessage(MessageBindingModel model);
        Task SaveMessage(string author, string recipient, string text);
        Task ReadMessagesFromDialog(string author, string recipient);
        Task<IEnumerable<MessageReturnModel>> GetUnreadMessagesFromDialog(string author, string recipient);
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
                Task<UserReturnModel> getAuthorTask = _userService.GetUserByLogin(author);
                Task<UserReturnModel> getRecipientTask = _userService.GetUserByLogin(recipient);
                await Task.WhenAll(getAuthorTask, getRecipientTask);

                Message message = new Message
                {
                    Author_Id = getAuthorTask.Result.Id,
                    Recipient_Id = getRecipientTask.Result.Id,
                    Time = DateTime.UtcNow,
                    WasRead = false,
                    Text = text,
                    State = EntityState.Added
                };
                context.Messages.Add(message);
                await context.SaveChangesAsync();
            }
        }
        public async Task SaveMessage(MessageBindingModel model)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var Recipient_Id = (await context.AspNetUsers.Where(x => x.UserName == model.RecipientUsername).FirstOrDefaultAsync()).Id;
                if (model.AuthorId == Recipient_Id)
                {
                    return;
                }
                Message message = new Message
                {
                    Author_Id = model.AuthorId,
                    Recipient_Id = Recipient_Id,
                    WasRead = false,
                    Time = DateTime.UtcNow,
                    Text = model.Text,
                    State = EntityState.Added
                };
                context.Messages.Add(message);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MessageReturnModel>> GetLastMessages(string author, string recipient,int count = 10)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var messages = await context.Messages
                    .Where(m => (m.Author.UserName == author && m.Recipient.UserName == recipient) ||
                                (m.Author.UserName == recipient && m.Recipient.UserName == author))
                    .OrderByDescending(m => m.Time)
                    .Take(count)
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
            if (mes.Text.Length >= 127)
            {
                mes.Text = mes.Text.Substring(0, 127) + "...";
            }
        }

        public async Task<IEnumerable<MessageReturnModel>> GetMessages(string author, string recipient, int count = 1, int offset = 10, int lastId = 1)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var messages = await context.Messages
                    .Where(m => (m.Author.UserName == author && m.Recipient.UserName == recipient) ||
                                (m.Author.UserName == recipient && m.Recipient.UserName == author))
                    .OrderByDescending(m => m.Time)
                    .Skip(offset)
                    .Take(count)
                    .OrderBy(m => m.Time)
                    .Select(m => new
                    {
                        Message = m,
                        m.Author,
                        m.Recipient
                    }).ToListAsync();
                var returnModel = messages.Select(s => _messageReturnModelMapper.Map(s.Message));
                return returnModel;
            }
        }

        public async Task ReadMessagesFromDialog(string author, string recipient)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var messages = await context.Messages
                   .Where(m => (m.Author.UserName == author && m.Recipient.UserName == recipient) ||
                               (m.Author.UserName == recipient && m.Recipient.UserName == author))
                   .Where(m => !m.WasRead)
                   .ToListAsync();
                messages.ForEach(m => { m.WasRead = true; m.State = EntityState.Modified; });
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<MessageReturnModel>> GetUnreadMessagesFromDialog(string author, string recipient)
        {
            using (ILiveChatContext context = _liveChatContextProvider.Context)
            {
                var messages = await context.Messages
                   .Where(m => (m.Author.UserName == author && m.Recipient.UserName == recipient) ||
                               (m.Author.UserName == recipient && m.Recipient.UserName == author))
                   .Where(m => !m.WasRead)
                   .OrderBy(m => m.Time)
                   .Select(m => new
                   {
                       Message = m,
                       m.Author,
                       m.Recipient
                   }).ToListAsync();
                var returnModel = messages.Select(s => _messageReturnModelMapper.Map(s.Message));
                return returnModel;
            }
        }
    }
}
