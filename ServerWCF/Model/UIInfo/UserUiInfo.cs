using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Model.UiInfo
{
    [DataContract]
    public class UserUiInfo : UiInfo
    {
        [DataMember]
        public int UserId { get; set; }
        
        public UserUiInfo(User user) : base( (user.FirstName + " " + user.LastName), user.Login, user.Status, user.ImageId)
        {
            UserId = user.Id;
        }
    }
}
