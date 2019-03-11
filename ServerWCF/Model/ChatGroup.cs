using ServerWCF.Model.Contacts;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ServerWCF.Model
{
    [DataContract]
    public class ChatGroup
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<UserToGroupContact> Participants { get; set; }
    }
}