using System;
using System.Collections.Generic;

namespace LiveChat.Hubs
{
    public class UserMessengerData
    {
        public string Name { get; set; }
        public DateTime ConnectedTime { get; set; }
        public List<string> ConnectionsIdList = new List<string>();
        //public string Avatar { get; set; }
    }
}