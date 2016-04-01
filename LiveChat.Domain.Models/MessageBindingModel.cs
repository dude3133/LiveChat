using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
