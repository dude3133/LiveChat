using LiveChat.DataAccess.Entities.Models;
using System.Data.Entity.ModelConfiguration;

namespace LiveChat.DataAccess.Configuration.Mapping
{
    public class AspNetUserMap : EntityTypeConfiguration<AspNetUser>
    {
        public AspNetUserMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(128);

            Property(t => t.Email)
                .HasMaxLength(256);

            Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(256);

            // Table & Column Mappings
            ToTable("AspNetUsers");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Email).HasColumnName("Email");
            Property(t => t.EmailConfirmed).HasColumnName("EmailConfirmed");
            Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            Property(t => t.SecurityStamp).HasColumnName("SecurityStamp");
            Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
            Property(t => t.PhoneNumberConfirmed).HasColumnName("PhoneNumberConfirmed");
            Property(t => t.TwoFactorEnabled).HasColumnName("TwoFactorEnabled");
            Property(t => t.LockoutEndDateUtc).HasColumnName("LockoutEndDateUtc");
            Property(t => t.LockoutEnabled).HasColumnName("LockoutEnabled");
            Property(t => t.AccessFailedCount).HasColumnName("AccessFailedCount");
            Property(t => t.UserName).HasColumnName("UserName");
        }
    }
}
