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

        [ForeignKey("SenderFK")]
        public User Sender { get; set; }

        [DataMember]
        public string SenderFK { get; set; }

        [DataMember]
        public DateTime DateOfSending { get; set; }

        public Message(int type, byte[] content, string senderFK)
        {
            Type = type;
            Content = content;
            SenderFK = senderFK;
            DateOfSending = new DateTime();
        }

    }
}
