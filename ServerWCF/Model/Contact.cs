using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ServerWCF.Model
{
    [DataContract]
    public class Contact
    {
        [Key]
        [DataMember]
        public int Id
        {
            get; set;
        }

        [InverseProperty("Contacts")]
        [DataMember]
        public User UserOwner
        {
            get; set;
        }

        [InverseProperty("Owners")]
        [DataMember]
        public User UserOwned
        {
            get; set;
        }
    }
}