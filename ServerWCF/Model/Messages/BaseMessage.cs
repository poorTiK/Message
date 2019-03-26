using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ServerWCF.Model.Messages
{
    [DataContract]
    [KnownType(typeof(UserMessage))]
    [KnownType(typeof(GroupMessage))]
    public abstract class BaseMessage
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        
        [DataMember]
        public int SenderId { get; set; }

        [ForeignKey("SenderId")]
        [DataMember]
        public User Sender { get; set; }

        [ForeignKey("ChatFile")]
        [DataMember]
        public virtual int ChatFileId { get; set; }

        [DataMember]
        public ChatFile ChatFile { get; set; }

        [DataMember]
        public byte[] Text { get; set; }

        [DataMember]
        public DateTime DateOfSending { get; set; }
    }
}