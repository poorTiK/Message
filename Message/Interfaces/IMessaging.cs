using Message.UserServiceReference;
using System.Collections.Generic;

namespace Message.Interfaces
{
    internal interface IMessaging : IView
    {
        List<MessageT> MessageList { get; set; }

        void UpdateMessageList();
    }
}