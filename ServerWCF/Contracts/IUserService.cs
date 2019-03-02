using ServerWCF.Model;
using ServerWCF.Model.Messages;
using System.Collections.Generic;
using System.ServiceModel;

namespace ServerWCF.Contracts
{
    [ServiceContract(CallbackContract = typeof(IUserCallback))]
    public interface IUserService
    {
        [OperationContract]
        List<User> GetAllUsers();

        [OperationContract]
        bool AddOrUpdateUser(User user);

        [OperationContract]
        User GetUser(string login, string password);

        [OperationContract]
        User GetUserByEmail(string email);

        [OperationContract]
        List<User> FindUsersByLogin(string login);

        [OperationContract]
        User GetUserByLogin(string login);

        [OperationContract]
        bool AddContact(User owner, User owned);

        [OperationContract]
        bool RemoveContact(User owner, User owned);

        [OperationContract]
        List<User> GetAllContacts(User owner);

        [OperationContract]
        bool IsExistsInContacts(User owner, User owned);

        [OperationContract]
        ApplicationSettings GetAppSettings(User user);

        [OperationContract]
        bool SaveAppSettings(ApplicationSettings appSettings);

        [OperationContract(IsOneWay = true)]
        void OnUserCame(User user);

        [OperationContract(IsOneWay = true)]
        void OnUserLeave(User user);

        [OperationContract(IsOneWay = true)]
        void SendMessage(BaseMessage message);

        [OperationContract]
        List<GroupMessage> GetGroupMessages(ChatGroup chatGroup, int limin);

        [OperationContract]
        List<UserMessage> GetUserMessages(User sender, User receiver, int limin);

        [OperationContract]
        List<BaseMessage> FindMessage(string keyWord);

        [OperationContract]
        bool EditMessage(BaseMessage editedMessage);
    }

    public interface IUserCallback
    {
        [OperationContract(IsOneWay = true)]
        void UserLeave(User user);

        [OperationContract(IsOneWay = true)]
        void UserCame(User user);

        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(BaseMessage message);
    }
}