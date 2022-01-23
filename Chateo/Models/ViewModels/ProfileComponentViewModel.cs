using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Models.ViewModels
{
    public class ProfileComponentViewModel
    {
        public IEnumerable<Chat> TopChats { get; set; }
        public User User { get; set; }
    }
}
