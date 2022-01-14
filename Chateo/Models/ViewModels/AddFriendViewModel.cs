using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Models.ViewModels
{
    public class AddFriendViewModel
    {
        public IEnumerable<User> NotFriends { get; set; }
        public IEnumerable<User> RequestsFrom { get; set; }
        public IEnumerable<User> RequestsTo { get; set; }
    }
}
