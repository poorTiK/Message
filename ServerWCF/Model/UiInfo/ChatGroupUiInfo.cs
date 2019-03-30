using System.Runtime.Serialization;

namespace ServerWCF.Model.UiInfo
{
    [DataContract]
    public class ChatGroupUiInfo : UiInfo
    {
        [DataMember]
        public int ChatGroupId { get; set; }

        public ChatGroupUiInfo(ChatGroup chatGroup) : base(chatGroup.Name, chatGroup.Name, "Group", chatGroup.ImageId)
        {
            ChatGroupId = chatGroup.Id;
        }
    }
}