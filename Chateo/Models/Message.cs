using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public byte[] Image { get; set; }
        public DateTime Date { get; set; }
        public bool Read { get; set; }

        public Message RepliedMessage { get; set; }
        public int? RepliedMessageId { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }

        public Chat Chat { get; set; }
        public int ChatId { get; set; }
    }
}
