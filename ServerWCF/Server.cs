using ServerWCF.Context;
using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServerWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Server : IServer
    {
        public bool AddNewUser(User UserThatShouldBeAdded)
        {
            using (UserContext db = new UserContext())
            {
                    db.Users.Add(UserThatShouldBeAdded);
                    db.SaveChanges();
                    return true;
            }
        }

        public User GetUser(string login, string password)
        {
            using (UserContext db = new UserContext())
            {
                try
                {

                    foreach (User user in db.Users)
                    {
                        if (user.Login == login && user.Password == password)
                            return user;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        //public CompositeType GetDataUsingDataContract(CompositeType composite)
        //{
        //    if (composite == null)
        //    {
        //        throw new ArgumentNullException("composite");
        //    }
        //    if (composite.BoolValue)
        //    {
        //        composite.StringValue += "Suffix";
        //    }
        //    return composite;
        //}
    }
}
