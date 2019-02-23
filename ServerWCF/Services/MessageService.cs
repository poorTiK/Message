using ServerWCF.Context;
using ServerWCF.Contracts;
using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerWCF.Services
{
    public class MessageService : IMessageService
    {
        private struct MessageData
        {
            public MessageT message;
            public IMessageCallback callback;
        }

        public void SendMessage(MessageT message)
        {
            using(UserContext userContext = new UserContext())
            {
                try
                {
                    User dbSender = userContext.Users.Where(u => u.Id == message.SenderId).First();
                    User dbReceiver = userContext.Users.Where(u => u.Id == message.ReceiverId).First();

                    message.Sender = dbSender;
                    message.Receiver = dbReceiver;

                    userContext.Messages.Add(message);
                    userContext.SaveChanges();

                    IMessageCallback callback = OperationContext.Current.GetCallbackChannel<IMessageCallback>();

                    MessageData messageData = new MessageData();
                    messageData.message = message;
                    messageData.callback = callback;

                    Thread t = new Thread(new ParameterizedThreadStart(ExecuteCallback));
                    t.IsBackground = true;
                    t.Start(messageData);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void ExecuteCallback(object mesDataObj)
        {
            MessageData messageData = (MessageData)mesDataObj;
            (messageData.callback).ReceiveMessage(messageData.message);
        }
    }
}
