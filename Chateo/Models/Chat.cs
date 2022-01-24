using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Models
{
    public enum ChatType
    {
        Private,
        Public
    }

    public class Chat
    {
        public int Id { get; set; }
        
        public string Title { get; set; }

        public ChatType ChatType { get; set; }

        public byte[] Avatar { get; set; }

        [NotMapped]
        public User OtherUser { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Message> Messages { get; set; }

        public Chat()
        {
            Users = new List<User>();
            Messages = new List<Message>();
        }
    }
}
