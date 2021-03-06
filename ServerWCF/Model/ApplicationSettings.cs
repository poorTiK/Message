﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ServerWCF.Model
{
    [DataContract]
    public class ApplicationSettings
    {
        [Key]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int WindowSize { get; set; }

        [DataMember]
        public string Language { get; set; }

        [DataMember]
        public string Theme { get; set; }

        [DataMember]
        public bool AllowNotifications { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        [DataMember]
        public User User { get; set; }
    }
}