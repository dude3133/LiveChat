using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace LiveChat.DataAccess.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            State = EntityState.Unchanged;
        }

        [NotMapped]
        public EntityState State { get; set; }
    }
}
