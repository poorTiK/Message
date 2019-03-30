using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ServerWCF.Model.Contacts
{
    [DataContract]
    [KnownType(typeof(UserToUserContact))]
    [KnownType(typeof(UserToGroupContact))]
    public abstract class BaseContact
    {
        [Key]
        [DataMember]
        public int Id
        {
            get; set;
        }

        [DataMember]
        public int UserOwnerId
        {
            get; set;
        }

        [ForeignKey("UserOwnerId")]
        [InverseProperty("Contacts")]
        [DataMember]
        public User UserOwner
        {
            get; set;
        }
    }
}