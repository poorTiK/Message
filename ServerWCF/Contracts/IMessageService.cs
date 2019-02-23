using ServerWCF.Model;
using System.ServiceModel;

namespace ServerWCF.Contracts
{
    [ServiceContract(CallbackContract = typeof(IMessageCallback))]
    public interface IMessageService
    {
        [OperationContract(IsOneWay = true)]
        void SendMessage(MessageT message);
    }

    public interface IMessageCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(MessageT message);
    }
}
