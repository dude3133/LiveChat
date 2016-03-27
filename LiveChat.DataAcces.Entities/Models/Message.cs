using System;

namespace LiveChat.DataAcces.Entities.Models
{
    public partial class Message : BaseEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public string Author_Id { get; set; }
        public string Recipient_Id { get; set; }
        public bool IsDeletedByAuthor { get; set; }
        public bool IsDeletedByRecipient { get; set; }
        public bool WasRead { get; set; }
        public virtual AspNetUser Author { get; set; }
        public virtual AspNetUser Recipient { get; set; }
    }
}
