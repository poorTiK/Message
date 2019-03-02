using ServerWCF.Model.Messages;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ServerWCF.Model
{
    [DataContract]
    public class UserMessage : BaseMessage
    {
        [DataMember]
        public int ReceiverId { get; set; }

        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; }
    }
}