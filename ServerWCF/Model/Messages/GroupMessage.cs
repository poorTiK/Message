using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ServerWCF.Model.Messages
{
    [DataContract]
    public class GroupMessage : BaseMessage
    {
        [DataMember]
        public int ChatGroupId { get; set; }

        [ForeignKey("ChatGroupId")]
        public ChatGroup ChatGroup { get; set; }
    }
}