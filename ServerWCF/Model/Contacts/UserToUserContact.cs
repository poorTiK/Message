using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ServerWCF.Model.Contacts
{
    [DataContract]
    public class UserToUserContact : BaseContact
    {
        [DataMember]
        public int UserOwnedId { get; set; }

        [ForeignKey("UserOwnedId")]
        [InverseProperty("Owners")]
        [DataMember]
        public User UserOwned
        {
            get; set;
        }
    }
}