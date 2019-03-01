using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
