using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Contracts
{
    [ServiceContract]
    public interface IContactsService
    {
        [OperationContract]
        List<Contact> GetContacts(User user);
    }
}
