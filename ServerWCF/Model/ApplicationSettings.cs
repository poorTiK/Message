﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Model
{
    [DataContract]
    public class ApplicationSettings
    {
        
        [ForeignKey("UserFK")]
        public User User { get; set; }

        [DataMember]
        public string UserFK { get; set; }

        [DataMember]
        public int WindowSize { get; set; }

        [DataMember]
        public int Language { get; set; }

        public ApplicationSettings(int windowSize, int language)
        {
            WindowSize = windowSize;
            Language = language;
        }
    }
}
