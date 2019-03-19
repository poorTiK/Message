using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
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
        public int ImageId { get; set; }

        [ForeignKey("ImageId")]
        public ChatFile Image { get; set; }

        [DataMember]
        [NotMapped]
        public Image UiImage { get; set; }

        [DataMember]
        public bool IsSelected{ get; set; }

        protected UiInfo(string name, string uniqueName, string status, int imageId)
        {
            Name = name;
            UniqueName = uniqueName;
            Status = status;
            ImageId = imageId;
        }
    }
}
