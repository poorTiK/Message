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
    public class MessageT
    {
        [Key]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int SenderId { get; set; }

        [ForeignKey("SenderId")]
        public User Sender { get; set; }

        [DataMember]
        public int ReceiverId { get; set; }

        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public byte[] Content { get; set; }

        [DataMember]
        public DateTime DateOfSending { get; set; }

        public MessageT()
        {
            Type = "TEXT";
            Content = new byte[0];
            DateOfSending = DateTime.Now;
        }

        public MessageT(string type, byte[] content, User sender, User reveiver)
        {
            Type = type;
            Content = content;
            Sender = sender;
            Receiver = reveiver;
            DateOfSending = DateTime.Now;
        }

    }
}
