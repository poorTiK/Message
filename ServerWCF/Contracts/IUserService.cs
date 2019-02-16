using ServerWCF.Model;
using System.Collections.Generic;
using System.ServiceModel;

namespace ServerWCF.Contracts
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        bool AddNewUser(User user);

        [OperationContract]
        User GetUser(string login, string password);

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

        //[OperationContract]
        //CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "ServerWCF.ContractType".
    //[DataContract]
    //public class CompositeType
    //{
    //    bool boolValue = true;
    //    string stringValue = "Hello ";

    //    [DataMember]
    //    public bool BoolValue
    //    {
    //        get { return boolValue; }
    //        set { boolValue = value; }
    //    }

    //    [DataMember]
    //    public string StringValue
    //    {
    //        get { return stringValue; }
    //        set { stringValue = value; }
    //    }
    //}
}
