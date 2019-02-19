using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Key]
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public int WindowSize { get; set; }

        [DataMember]
        public int Language { get; set; }

        [DataMember]
        public string Theme { get; set; }

        [DataMember]
        public int UserId{ get; set; }

        [DataMember]
        public bool AllowNotifications { get; set; }

        [ForeignKey("UserId")]
        [DataMember]
        public User User { get; set; }

        public ApplicationSettings(int windowSize, int language, string theme)
        {
            WindowSize = windowSize;
            Language = language;
            Theme = theme;
        }
    }
}
