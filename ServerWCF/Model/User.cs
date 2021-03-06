﻿using ServerWCF.Model.Contacts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Runtime.Serialization;

namespace ServerWCF.Model
{
    [DataContract]
    public class User
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public byte[] Password { get; set; }

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

        [ForeignKey("Image")]
        [DataMember]
        public virtual int ImageId { get; set; }

        public ChatFile Image { get; set; }

        [DataMember]
        [NotMapped]
        public Image UiImage { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public List<BaseContact> Contacts { get; set; }

        [DataMember]
        public List<UserToUserContact> Owners { get; set; }

        [DataMember]
        [NotMapped]
        public int UnreadMessageCount { get; set; }
    }
}