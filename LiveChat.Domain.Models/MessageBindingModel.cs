using System;
using System.ComponentModel.DataAnnotations;

namespace LiveChat.Domain.Models
{
    public class MessageBindingModel
    {
        [Required]
        [MaxLength(2000)]
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public string AuthorId { get; set; }
        [Required]
        public string RecipientUsername { get; set; }
    }
}
