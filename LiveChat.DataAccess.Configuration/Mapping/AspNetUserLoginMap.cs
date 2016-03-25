using System.Data.Entity.ModelConfiguration;
using LiveChat.DataAcces.Entities.Models;

namespace LiveChat.DataAccess.Configuration.Mapping
{
    public class AspNetUserLoginMap : EntityTypeConfiguration<AspNetUserLogin>
    {
        public AspNetUserLoginMap()
        {
            // Primary Key
            HasKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId });

            // Properties
            Property(t => t.LoginProvider)
                .IsRequired()
                .HasMaxLength(128);

            Property(t => t.ProviderKey)
                .IsRequired()
                .HasMaxLength(128);

            Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            ToTable("AspNetUserLogins");
            Property(t => t.LoginProvider).HasColumnName("LoginProvider");
            Property(t => t.ProviderKey).HasColumnName("ProviderKey");
            Property(t => t.UserId).HasColumnName("UserId");

            // Relationships
            HasRequired(t => t.AspNetUser)
                .WithMany(t => t.AspNetUserLogins)
                .HasForeignKey(d => d.UserId);

        }
    }
}
