using System;

namespace LiveChat.Domain.Models
{
    public class FriendRequestReturnModel
    {
        public DateTime Date { get; set; }
        public UserReturnModel Receiver { get; set; }
        public UserReturnModel Requestor { get; set; }
    }
}
