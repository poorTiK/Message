using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Model.Contacts
{
    [DataContract]
    [KnownType(typeof(UserToUserContact))]
    [KnownType(typeof(UserToGroupContact))]
    public class BaseContact
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
