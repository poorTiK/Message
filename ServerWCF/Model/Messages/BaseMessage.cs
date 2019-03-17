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
        public User Sender { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public byte[] Content { get; set; }

        [DataMember]
        public string AdditionalInfo { get; set; }

        [DataMember]
        public DateTime DateOfSending { get; set; }
    }
}