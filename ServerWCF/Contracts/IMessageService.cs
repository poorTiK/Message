using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Contracts
{
    [ServiceContract(CallbackContract = typeof(IClientCallback))]
    public interface IMessageService
    {
        [OperationContract(IsOneWay = true)]
        void SendMessage(MessageT message);
    }

    public interface IClientCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(MessageT message);
    }
}
