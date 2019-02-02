using ServerWCF.Context;
using ServerWCF.Contracts;
using ServerWCF.Model;
using System;

namespace ServerWCF.Services
{
    public class UserService : IUserService
    {
        public bool AddNewUser(User user)
        {
            using (UserContext db = new UserContext())
            {
                //try
                //{
                    db.Users.Add(user);
                    db.SaveChanges();
                    return true;
                //}
                //catch (Exception)
                //{
                //    return false;
                //    throw;
                //}
            }
        }

        public User GetUser(string login, string password)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    foreach (var user in db.Users)
                    {
                        if (user.Login == login && user.Password == password)
                            return user;
                    }
                    return null;
                }
                catch (Exception)
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
