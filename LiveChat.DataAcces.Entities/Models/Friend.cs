namespace LiveChat.DataAccess.Entities.Models
{
    public partial class Friend :BaseEntity
    {
        public string Requestor_Id { get; set; }
        public string Receiver_Id { get; set; }
        public System.DateTime Date { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
    }
}
