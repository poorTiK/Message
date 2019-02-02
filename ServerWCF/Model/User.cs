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
        public DateTime LastOnline { get; set; }

        [DataMember]
        public ApplicationSettings ApplicationSettings { get; set; }

        [DataMember]
        public ICollection<MessageT> Messages { get; set; }

        [DataMember]
        public ICollection<User> Owners { get; set; }

        [DataMember]
        public ICollection<User> Contacts { get; set; }

        public User(string firstName, string loginId, string password, string email)
        {
            Login = loginId;
            Password = password;
            FirstName = firstName;
            LastOnline = DateTime.Now;
        }

        public User()
        {
            Login = "";
            Password = "";
            FirstName = "";
            LastName = "";
            Phone = "";
            Email = "";
            Bio = "";
            Avatar = new byte[0];
            LastOnline = DateTime.Now;
        }
    }
}
