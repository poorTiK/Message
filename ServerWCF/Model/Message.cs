using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Model
{
    [DataContract]
    public class Message
    {
        [Key]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public byte[] Content { get; set; }

        [ForeignKey("User")]
        [DataMember]
        public int SenderId { get; set; }

        [DataMember]
        public DateTime DateOfSending { get; set; }

        public Message(int type, byte[] content, int senderId)
        {
            Type = type;
            Content = content;
            SenderId = senderId;
            DateOfSending = new DateTime();
        }

    }
}
