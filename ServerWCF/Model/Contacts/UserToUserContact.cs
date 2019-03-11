﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ServerWCF.Model.Contacts
{
    [DataContract]
    public class UserToUserContact : BaseContact
    {
        [InverseProperty("Owners")]
        [DataMember]
        public User UserOwned
        {
            get; set;
        }
    }
}