using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Model.Contacts
{
    [DataContract]
    public class UserToGroupContact : BaseContact
    {

        [InverseProperty("Participants")]
        [DataMember]
        public ChatGroup ChatGroup
        {
            get; set;
        }
    }
}
