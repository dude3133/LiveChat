using System.Collections.Generic;

namespace LiveChat.DataAcces.Entities.Models
{
    public class AspNetRole
    {
        public AspNetRole()
        {
            AspNetUsers = new List<AspNetUser>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}
