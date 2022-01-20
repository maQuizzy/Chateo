using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Models.ViewModels
{
    public class ContactsViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public AddFriendViewModel AddFriendViewModel { get;set; }
    }
}
