using ServerWCF.Model.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Model
{
    public class ChatGroup
    {
        public int Id { get; set; }

        public List<User> Participants { get; set; }
    }
}
