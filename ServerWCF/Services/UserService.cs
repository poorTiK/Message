using ServerWCF.Context;
using ServerWCF.Contracts;
using ServerWCF.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ServerWCF.Services
{
    public class UserService : IUserService
    {
        public User AddContact(User owner, User contact)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    db.Users.First(x => x.Login == owner.Login).Contacts.Add(contact);
                    db.SaveChanges();

                    return owner;
                }
                catch (Exception)
                {
                    return null;
                    throw;
                }
            }
        }

        public bool AddNewUser(User user)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                    throw;
                }
            }
        }

        public ICollection<User> GetAllContacts(User owner)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    //ICollection<User> contacts = db.Users.Where(x => x.Owners.Contains(owner)).ToList();

                    if (contacts != null)
                    {
                        return contacts;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                    throw;
                }
            }
        }

        public ICollection<User> GetAllUsers()
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    return db.Users.ToList();
                }
                catch (Exception)
                {
                    return null;
                    throw;
                }
            }
        }

        public ICollection<User> GetAllUsersByLogin(string login)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    return db.Users.Where(x => x.Login.Contains(login)).ToList();
                }
                catch (Exception)
                {
                    return null;
                    throw;
                }
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
