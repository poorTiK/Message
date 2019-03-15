using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Model.UiInfo
{
    [DataContract]
    [KnownType(typeof(UserUiInfo))]
    [KnownType(typeof(ChatGroupUiInfo))]
    public abstract class UiInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string UniqueName { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public byte[] Avatar { get; set; }

        protected UiInfo(string name, string uniqueName, string status, byte[] avatar)
        {
            Name = name;
            UniqueName = uniqueName;
            Status = status;
            Avatar = avatar;
        }
    }
}
