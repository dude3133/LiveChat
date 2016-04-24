using LiveChat.DataAccess.Entities.Models;
using System.Data.Entity.ModelConfiguration;

namespace LiveChat.DataAccess.Configuration.Mapping
{
    public class FriendRequestMap : EntityTypeConfiguration<FriendRequest>
    {
        public FriendRequestMap()
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
            this.ToTable("FriendRequest");
            this.Property(t => t.Requestor_Id).HasColumnName("Requestor_Id");
            this.Property(t => t.Receiver_Id).HasColumnName("Receiver_Id");
            this.Property(t => t.Date).HasColumnName("Date");

            // Relationships
            this.HasRequired(t => t.Receiver)
                .WithMany(t => t.FriendRequests)
                .HasForeignKey(d => d.Receiver_Id);
            this.HasRequired(t => t.Requestor)
                .WithMany(t => t.FriendRequests1)
                .HasForeignKey(d => d.Requestor_Id);

        }
    }
}
