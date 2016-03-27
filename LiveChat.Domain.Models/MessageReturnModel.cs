using System;
using LiveChat.DataAcces.Entities.Models;

namespace LiveChat.Domain.Models
{
    public class MessageReturnModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public AspNetUser UserAuthor { get; set; }
        public AspNetUser UserRecipient { get; set; }
    }
}
