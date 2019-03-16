using Message.UserServiceReference;
using System.Collections.Generic;

namespace Message.Interfaces
{
    internal interface IMessaging : IView
    {
        List<BaseMessage> MessageList { get; set; }

        void UpdateMessageList();
    }
}