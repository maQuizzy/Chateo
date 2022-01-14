using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Models
{
    public class Friendship
    {
        public User User { get; set; }
        public string UserId { get; set; }

        public User UserFriend { get; set; }
        public string UserFriendId { get; set; }
    }
}
