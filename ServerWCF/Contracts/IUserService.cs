using ServerWCF.Model;
using System.Collections.Generic;
using System.ServiceModel;

namespace ServerWCF.Contracts
{
    [ServiceContract(CallbackContract = typeof(IUserCallback))]
    public interface IUserService
    {
        [OperationContract]
        bool AddOrUpdateUser(User user);

        [OperationContract]
        User GetUser(string login, string password);

        [OperationContract]
        User GetUserByEmail(string email);

        [OperationContract]
        bool AddContact(User owner, User owned);

        [OperationContract]
        bool RemoveContact(User owner, User owned);

        [OperationContract]
        List<User> GetAllContacts(User owner);

        [OperationContract]
        List<User> GetAllUsers();

        [OperationContract]
        User GetUserByLogin(string login);

        [OperationContract]
        List<MessageT> GetMessages(User sender, User receiver, int limin);

        [OperationContract]
        List<MessageT> FindMessage(string keyWord);

        [OperationContract]
        ApplicationSettings getAppSettings(User user);

        [OperationContract]
        bool saveAppSettings(ApplicationSettings appSettings);

        [OperationContract]
        List<User> FindUsersByLogin(string login);

        [OperationContract]
        bool IsExistsInContacts(User owner, User owned);

        [OperationContract(IsOneWay = true)]
        void onUserCame(User user);

        [OperationContract(IsOneWay = true)]
        void onUserLeave(User user);

        [OperationContract(IsOneWay = true)]
        void SendMessage(MessageT message);
    }
 
    public interface IUserCallback
    {
        [OperationContract(IsOneWay = true)]
        void UserLeave(User user);

        [OperationContract(IsOneWay = true)]
        void UserCame(User user);

        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(MessageT message);
    }
}
