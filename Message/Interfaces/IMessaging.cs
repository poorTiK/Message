using Message.MessageServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Interfaces
{
    interface IMessaging: IView
    {
        List<MessageT> MessageList { get; set; }

        void UpdateMessageList();
    }
}
