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
        public bool AddContact(User owner, User owned)
        {
            using (UserContext userContext = new UserContext())
            {
                    Contact contact = new Contact();

                    User ownerFromDb = userContext.Users.Where(dbUser => dbUser.Login == owner.Login).First();
                    User ownedFromDb = userContext.Users.Where(dbUser => dbUser.Login == owned.Login).First();

                    contact.UserOwner = ownerFromDb;
                    contact.UserOwned = ownedFromDb;

                    userContext.Contacts.Add(contact);
                    userContext.SaveChanges();

                    return true;
            }
        }

        public bool RemoveContact(User owner, User owned)
        {
            using (UserContext userContext = new UserContext())
            {
                    List<Contact> contacts = userContext.Contacts.Include("UserOwner")
                        .Include("UserOwned")
                        .ToList();

                    User ownerFromDb = userContext.Users.Where(u => u.Login == owner.Login).First();
                    User ownedFromDb = userContext.Users.Where(u => u.Login == owned.Login).First();

                    foreach (Contact contact in contacts)
                    {
                        if (contact.UserOwner.Id == ownerFromDb.Id && contact.UserOwned.Id == ownedFromDb.Id)
                        {
                            userContext.Contacts.Remove(contact);
                            userContext.SaveChanges();
                            return true;
                        }
                    }

                    return false;
            }
        }

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

        public ICollection<User> GetAllContacts(User owner)
        {
            using (UserContext db = new UserContext())
            {
                List<User> contactsForOwner = null;
                //try
                //{
                    var userId = owner.Id;

                    contactsForOwner = db.Users.SqlQuery(" select * " +
                            "from Users " +
                            "where Users.Id in (select UserOwned_Id " +
                            "from Contacts " +
                            "where Contacts.UserOwner_Id = @p0);", userId).ToList();
                //}
                //catch (Exception ex)
                //{

                //}

                return contactsForOwner;
            }
            
        }

        public ICollection<User> GetAllUsers()
        {
            using (UserContext db = new UserContext())
            {
                //try
                //{
                    return db.Users.ToList();
                //}
                //catch (Exception)
                //{
                //    return null;
                //    throw;
                //}
            }
        }

        public ICollection<User> GetAllUsersByLogin(string login)
        {
            using (UserContext db = new UserContext())
            {
                //try
                //{
                    return db.Users.Where(x => x.Login.Contains(login)).ToList();
                //}
                //catch (Exception)
                //{
                //    return null;
                //    throw;
                //}
        }
        }

        public User GetUser(string login, string password)
        {
            using (UserContext db = new UserContext())
            {
                //try
                //{
                    foreach (var user in db.Users)
                    {
                        if (user.Login == login && user.Password == password)
                            return user;
                    }
                    return null;
                //}
                //catch (Exception)
                //{
                //    return null;
                //}
            }
        }

        public User GetUserByEmail(string email)
        {
            using (UserContext db = new UserContext())
            {
                foreach (var user in db.Users)
                {
                    if (user.Email == email)
                        return user;
                }
                return null;
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
