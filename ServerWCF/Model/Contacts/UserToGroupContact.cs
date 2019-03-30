using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ServerWCF.Model.Contacts
{
    [DataContract]
    public class UserToGroupContact : BaseContact
    {
        [DataMember]
        public int ChatGroupId { get; set; }

        [ForeignKey("ChatGroupId")]
        [InverseProperty("Participants")]
        [DataMember]
        public ChatGroup ChatGroup
        {
            get; set;
        }
    }
}