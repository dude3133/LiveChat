using LiveChat.DataAcces.Entities.Models;
using System.Data.Entity.ModelConfiguration;

namespace LiveChat.DataAccess.Configuration.Mapping
{
    public class MessageMap : EntityTypeConfiguration<Message>
    {
        public MessageMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Text)
                .IsRequired();

            this.Property(t => t.Author_Id)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Recipient_Id)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("Messages");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Text).HasColumnName("Text");
            this.Property(t => t.Time).HasColumnName("Time");
            this.Property(t => t.Author_Id).HasColumnName("Author_Id");
            this.Property(t => t.Recipient_Id).HasColumnName("Recipient_Id");
            this.Property(t => t.IsDeletedByAuthor).HasColumnName("IsDeletedByAuthor");
            this.Property(t => t.IsDeletedByRecipient).HasColumnName("IsDeletedByRecipient");
            this.Property(t => t.WasRead).HasColumnName("WasRead");

            // Relationships
            this.HasRequired(t => t.Author)
                .WithMany(t => t.SendMessages)
                .HasForeignKey(d => d.Author_Id);
            this.HasRequired(t => t.Recipient)
                .WithMany(t => t.ReceivedMessages)
                .HasForeignKey(d => d.Recipient_Id);

        }
    }
}
