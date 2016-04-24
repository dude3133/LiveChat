namespace LiveChat.DataAccess.Entities.Models
{
    public partial class FriendRequest :BaseEntity
    {
        public string Requestor_Id { get; set; }
        public string Receiver_Id { get; set; }
        public System.DateTime Date { get; set; }
        public virtual AspNetUser Receiver { get; set; }
        public virtual AspNetUser Requestor { get; set; }
    }
}
