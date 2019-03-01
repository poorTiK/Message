using ServerWCF.Model.Messages;
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
    public class UserMessage : BaseMessage
    {
        [DataMember]
        public int UserReceiverId { get; set; }

        [ForeignKey("UserReceiverId")]
        public User Receiver { get; set; }
    }
}
