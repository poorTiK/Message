using System.Runtime.Serialization;

namespace ServerWCF.Model
{
    [DataContract]
    public class ChatFile
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Extension { get; set; }

        [DataMember]
        public byte[] Source { get; set; }
    }
}