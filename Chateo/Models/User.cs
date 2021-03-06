using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chateo.Models
{
    public class User : IdentityUser
    {
        public ICollection<Chat> Chats { get; set; }
        public ICollection<Message> Messages { get; set; }
        
        public ICollection<Friendship> FriendsOf { get; set; }
        public ICollection<Friendship> Friends { get; set; }

        public ICollection<FriendRequest> FriendRequestsOf { get; set; }
        public ICollection<FriendRequest> FriendRequests { get; set; }
    }
}
