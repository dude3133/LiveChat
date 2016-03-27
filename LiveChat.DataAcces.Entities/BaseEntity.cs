using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveChat.DataAcces.Entities
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
