﻿using LiveChat.DataAccess.Entities.Models;

namespace LiveChat.Domain.Models.Mappers
{
    public interface IMessageReturnModelMapper
    {
        MessageReturnModel Map(Message msg);
    }

    public class MessageReturnModelMapper : IMessageReturnModelMapper
    {
        private IUserReturnModelMapper userReturnModelMapper;

        public MessageReturnModelMapper(IUserReturnModelMapper _userReturnModelMapper)
        {
            userReturnModelMapper = _userReturnModelMapper;
        }

        public MessageReturnModel Map(Message msg)
        {
            return new MessageReturnModel()
            {
                Id = msg.Id,
                Text = msg.Text,
                Time = msg.Time,
                UserAuthor = userReturnModelMapper.Map(msg.Author),
                UserRecipient = userReturnModelMapper.Map(msg.Recipient)
            };
        }
    }
}
