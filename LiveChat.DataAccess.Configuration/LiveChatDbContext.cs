using System.Data.Entity;
using LiveChat.DataAccess.Configuration.Mapping;
using LiveChat.DataAcces.Entities.Models;

namespace LiveChat.DataAccess.Configuration
{
    public interface ILiveChatContext
    {
        DbSet<AspNetRole> AspNetRoles { get; set; }
        DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        DbSet<AspNetUser> AspNetUsers { get; set; }
    }

    public partial class LiveChatContext : DbContext, ILiveChatContext
    {
        static LiveChatContext()
        {
            Database.SetInitializer<LiveChatContext>(null);
        }

        public LiveChatContext()
            : base("Name=LiveChatDbContext")
        {
        }

        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AspNetRoleMap());
            modelBuilder.Configurations.Add(new AspNetUserClaimMap());
            modelBuilder.Configurations.Add(new AspNetUserLoginMap());
            modelBuilder.Configurations.Add(new AspNetUserMap());
        }
    }
}
