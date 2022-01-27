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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsOnline { get; set; }
        public byte[] Avatar { get; set; }

        public ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
        public ICollection<Chat> Chats { get; set; } = new List<Chat>();
        public ICollection<Message> Messages { get; set; } = new List<Message>(); 
        
        public ICollection<Friendship> FriendsOf { get; set; } = new List<Friendship>(); 
        public ICollection<Friendship> Friends { get; set; } = new List<Friendship>(); 

        public ICollection<FriendRequest> FriendRequestsOf { get; set; } = new List<FriendRequest>(); 
        public ICollection<FriendRequest> FriendRequests { get; set; } = new List<FriendRequest>(); 
    }
}
