﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Model.UiInfo
{
    [DataContract]
    public class ChatGroupUiInfo : UiInfo
    {
        [DataMember]
        public int ChatGroupId { get; set; }
    }
}
