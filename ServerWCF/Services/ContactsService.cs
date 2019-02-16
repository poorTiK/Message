using ServerWCF.Context;
using ServerWCF.Contracts;
using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Services
{
    public class ContactsService : IContactsService
    {
        public List<Contact> GetContacts(User user)
        {
            using (UserContext context = new UserContext())
            {
                foreach(User innerUser in context.Users.Include("Contacts"))
                {
                    if(innerUser.Login == user.Login)
                    {
                        return innerUser.Contacts;
                    }
                }
                return null;
            }
        }
    }
}
