using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Model.Messages
{
    [DataContract]
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
        public DateTime DateOfSending { get; set; }
    }
}
