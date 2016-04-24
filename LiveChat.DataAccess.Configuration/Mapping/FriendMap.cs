using LiveChat.DataAccess.Entities.Models;
using System.Data.Entity.ModelConfiguration;

namespace LiveChat.DataAccess.Configuration.Mapping
{
    public class FriendMap : EntityTypeConfiguration<Friend>
    {
        public FriendMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Requestor_Id, t.Receiver_Id});

            // Properties
            this.Property(t => t.Requestor_Id)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Receiver_Id)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("Friends");
            this.Property(t => t.Requestor_Id).HasColumnName("Requestor_Id");
            this.Property(t => t.Receiver_Id).HasColumnName("Receiver_Id");
            this.Property(t => t.Date).HasColumnName("Date");

            // Relationships
            this.HasRequired(t => t.AspNetUser)
                .WithMany(t => t.Friends)
                .HasForeignKey(d => d.Receiver_Id);
            this.HasRequired(t => t.AspNetUser1)
                .WithMany(t => t.Friends1)
                .HasForeignKey(d => d.Requestor_Id);

        }
    }
}
