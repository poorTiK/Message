using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ServerWCF.Model
{
    [DataContract]
    public class User
    {
        [Key]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Bio { get; set; }

        [DataMember]
        public byte[] Avatar { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public List<Contact> Owners { get; set; }

        [DataMember]
        public List<Contact> Contacts { get; set; }

        [DataMember]
        [NotMapped]
        public int UnreadMessageCount { get; set; }

    }
}
