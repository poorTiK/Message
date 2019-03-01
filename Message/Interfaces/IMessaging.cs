using Message.UserServiceReference;
using System.Collections.Generic;

namespace Message.Interfaces
{
    internal interface IMessaging : IView
    {
        List<UserMessage> MessageList { get; set; }

        void UpdateMessageList();
    }
}