using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Model
{
    [DataContract]
    public class User
    {
        [Key]
        [DataMember]
        public string LoginId { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string ShownName { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Bio { get; set; }

        [DataMember]
        public byte[] Avatar { get; set; }

        [DataMember]
        public DateTime LastOnline { get; set; }

        public User(string loginId, string password, string shownName)
        {
            LoginId = loginId;
            Password = password;
            ShownName = shownName;
        }
    }
}
