﻿using LiveChat.DataAcces.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.Domain.Models.Mappers
{
    public interface IMessageReturnModelMapper
    {
        MessageReturnModel Map(Message msg);
    }

    public class MessageReturnModelMapper : IMessageReturnModelMapper
    {


        public MessageReturnModel Map(Message msg)
        {
            return new MessageReturnModel()
            {
                Id = msg.Id,
                Text = msg.Text,
                Time = msg.Time,
                UserAuthor = msg.Author,
                UserRecipient = msg.Recipient
            };
        }
    }
}