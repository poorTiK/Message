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
        public int Type { get; set; }

        [DataMember]
        public byte[] Content { get; set; }

        [DataMember]
        [ForeignKey("Sender")]
        public int SenderId { get; set; }

        public User Sender { get; set; }

        [DataMember]
        public DateTime DateOfSending { get; set; }

        public MessageT(int type, byte[] content, User sender)
        {
            Type = type;
            Content = content;
            Sender = sender;
            DateOfSending = new DateTime(2015, 7, 20);
        }

    }
}
