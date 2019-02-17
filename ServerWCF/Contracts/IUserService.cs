using ServerWCF.Model;
using System.Collections.Generic;
using System.ServiceModel;

namespace ServerWCF.Contracts
{
    [ServiceContract]
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
        ICollection<User> GetAllContacts(User owner);

        [OperationContract]
        ICollection<User> GetAllUsers();

        [OperationContract]
        ICollection<User> GetAllUsersByLogin(string login);

        [OperationContract]
        List<MessageT> GetMessages(User sender, User receiver, int limin);

        [OperationContract]
        List<MessageT> FindMessage(string keyWord);
    }
}
