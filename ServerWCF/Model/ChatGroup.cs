using ServerWCF.Model.Contacts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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


        [ForeignKey("Image")]
        [DataMember]
        public int ImageId { get; set; }

        public ChatFile Image { get; set; }

        [DataMember]
        public List<UserToGroupContact> Participants { get; set; }
    }
}