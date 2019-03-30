using ServerWCF.Model;
using ServerWCF.Model.Messages;
using ServerWCF.Model.UiInfo;
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
        List<UiInfo> GetAllUsersUiInfo();

        [OperationContract]
        string AddOrUpdateUser(User user);

        [OperationContract]
        User GetUser(string login, byte[] password);

        [OperationContract]
        User GetUserByEmail(string email);

        [OperationContract]
        List<User> FindNewUsersByLogin(int userId, string keyWorkForLogin);

        [OperationContract]
        List<UiInfo> FindNewUsersUiUnfoByLogin(int userId, string keyWorkForLogin);

        [OperationContract]
        User GetUserByLogin(string login);

        [OperationContract]
        User GetUserById(int id);

        [OperationContract]
        string AddOrUpdateChatGroup(ChatGroup chatGroupToAdd);

        [OperationContract]
        ChatGroup GetChatGroup(string chatGroupName);

        [OperationContract]
        bool AddUserToUserContact(int id_owner, int id_owned);

        [OperationContract]
        bool AddUserToChatGroupContact(int chatGroupId, int participantId);

        [OperationContract]
        bool RemoveUserToUserContact(int ownerId, int ownedId);

        [OperationContract]
        bool RemoveUserToChatGroupContact(int chatGroupId, int participantId);

        [OperationContract]
        List<User> GetAllUsersContacts(int userId);

        [OperationContract]
        List<ChatGroup> GetAllChatGroupsContacts(int userId);

        [OperationContract]
        List<UiInfo> GetAllContactsUiInfo(int id);

        [OperationContract]
        bool IsExistsInContacts(int id_owner, int id_owned);

        [OperationContract]
        ApplicationSettings GetAppSettings(int userId);

        [OperationContract]
        bool SaveAppSettings(ApplicationSettings appSettings);

        [OperationContract(IsOneWay = true)]
        void OnUserCame(int userId);

        [OperationContract(IsOneWay = true)]
        void OnUserLeave(int userId);

        [OperationContract]
        List<GroupMessage> GetGroupMessages(int chatGroupId, int limin);

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