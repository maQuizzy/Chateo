using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chateo.Models
{
    public class FriendRequest
    {
        public User UserFrom { get; set; }
        public string UserFromId { get; set; }

        public User UserTo { get; set; }
        public string UserToId { get; set; }
    }
}
