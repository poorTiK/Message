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
        [ForeignKey("User")]
        public int UserId { get; set; }

        public int WindowSize { get; set; }

        public int Language { get; set; }

        public User User { get; set; }

        public ApplicationSettings(int windowSize, int language)
        {
            WindowSize = windowSize;
            Language = language;
        }
    }
}
