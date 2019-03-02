using System.Collections.Generic;

namespace ServerWCF.Model
{
    public class ChatGroup
    {
        public int Id { get; set; }

        public List<User> Participants { get; set; }
    }
}