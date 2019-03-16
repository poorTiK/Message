using ServerWCF.Model;
using ServerWCF.Model.Contacts;
using ServerWCF.Model.Messages;
using ServerWCF.Model.UiInfo;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ServerWCF.Contracts
{
    [ServiceContract(CallbackContract = typeof(IUserCallback))]
    public interface IUserService : IPhotoService
    {
        [OperationContract]
        List<User> GetAllUsers();

        [OperationContract]
        List<UiInfo> GetAllUsersUiInfo();

        [OperationContract]
        string AddOrUpdateUser(User user);

        [OperationContract]
        User GetUser(string login, byte[] password);

        [OperationContract]
        User GetUserByEmail(string email);

        [OperationContract]
        List<User> FindUsersByLogin(string login);

        [OperationContract]
        List<UiInfo> FindUsersUiUnfoByLogin(string keyWorkForLogin);

        [OperationContract]
        User GetUserByLogin(string login);

        [OperationContract]
        User GetUserById(int id);


        [OperationContract]
        bool AddContact(int id_owner, int id_owned);

        [OperationContract]
        bool RemoveContact(User owner, User owned);

        [OperationContract]
        List<User> GetAllContacts(int id);

        [OperationContract]
        List<UiInfo> GetAllContactsUiInfo(int id);


        [OperationContract]
        bool IsExistsInContacts(int id_owner, int id_owned);


        [OperationContract]
        ApplicationSettings GetAppSettings(User user);

        [OperationContract]
        bool SaveAppSettings(ApplicationSettings appSettings);


        [OperationContract(IsOneWay = true)]
        void OnUserCame(User user);

        [OperationContract(IsOneWay = true)]
        void OnUserLeave(User user);


        [OperationContract]
        List<GroupMessage> GetGroupMessages(ChatGroup chatGroup, int limin);

        [OperationContract]
        List<UserMessage> GetUserMessages(int sender, int receiver, int limin);

        [OperationContract]
        List<BaseMessage> FindMessage(string keyWord);

        [OperationContract(IsOneWay = true)]
        void SendMessage(BaseMessage message);

        [OperationContract(IsOneWay = true)]
        void EditMessage(BaseMessage editedMessage);

        [OperationContract(IsOneWay = true)]
        void RemoveMessage(BaseMessage removedMessage);
    }

    public interface IUserCallback
    {
        [OperationContract(IsOneWay = true)]
        void UserLeave(User user);

        [OperationContract(IsOneWay = true)]
        void UserCame(User user);

        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(BaseMessage message);

        [OperationContract(IsOneWay = true)]
        void OnMessageRemoved(BaseMessage message);

        [OperationContract(IsOneWay = true)]
        void OnMessageEdited(BaseMessage message);
    }
}