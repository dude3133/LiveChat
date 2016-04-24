using Microsoft.AspNet.Identity.EntityFramework;
using LiveChat.DataAccess.Entities;

namespace LiveChat.DataAccess.Configuration
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
