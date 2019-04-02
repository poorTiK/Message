using Message.UserServiceReference;
using System;
using System.Collections.Generic;

namespace Message.Interfaces
{
    internal interface IMessaging : IView
    {
        List<BaseMessage> MessageList { get; set; }

        Action ScrolledToTop { get; set; }

        void UpdateMessageList();
    }
}