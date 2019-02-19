using ServerWCF.Context;
using ServerWCF.Contracts;
using ServerWCF.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace ServerWCF.Services
{
    public class UserService : IUserService
    {
        public bool AddContact(User owner, User owned)
        {
            using (UserContext userContext = new UserContext())
            {
                try
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
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public bool RemoveContact(User owner, User owned)
        {
            using (UserContext userContext = new UserContext())
            {
                try
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

                    return true;
                } catch (Exception ex)
                {
                    return false;
                }
                    
            }
        }

        public bool AddOrUpdateUser(User user)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    User result = db.Users.FirstOrDefault(u => u.Login == user.Login);

                    if (result != null)
                    { 
                        result.Email = user.Email;
                        result.Bio = user.Bio;
                        result.Avatar = user.Avatar;
                        result.FirstName = user.FirstName;
                        result.LastOnline = user.LastOnline;
                        result.Phone = user.Phone;
                        result.Password = user.Password;
                    }
                    else
                    {
                        db.Users.Add(user);
                    }

                    db.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
        }

        public ICollection<User> GetAllContacts(User owner)
        {
            using (UserContext db = new UserContext())
            {
                List<User> contactsForOwner = null;
                try
                {
                    var userId = owner.Id;

                    contactsForOwner = db.Users.SqlQuery(" select * " +
                            "from Users " +
                            "where Users.Id in (select UserOwned_Id " +
                            "from Contacts " +
                            "where Contacts.UserOwner_Id = @p0);", userId).ToList();
                }
                catch (Exception ex)
                {

                }

                return contactsForOwner;
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

        public User GetUserByEmail(string email)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    foreach (var user in db.Users)
                    {
                        if (user.Email == email)
                            return user;
                    }
                    return null;
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
        }

        public List<MessageT> GetMessages(User sender, User receiver, int limin)
        {
            using(UserContext context = new UserContext())
            {
                try
                {
                    List<MessageT> messagesToReturn = new List<MessageT>();
                    foreach (MessageT message in context.Messages.Include("Sender").Include("Receiver"))
                    {
                        if (messagesToReturn.Count == limin)
                        {
                            break;
                        }

                        if (message.Sender.Login == sender.Login &&
                            message.Receiver.Login == receiver.Login)
                        {
                            messagesToReturn.Add(message);
                        }

                        if(message.Sender.Login == receiver.Login && 
                            message.Receiver.Login == sender.Login)
                        {
                            messagesToReturn.Add(message);
                        }
                    }
                    return messagesToReturn;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<MessageT> FindMessage(string keyWord)
        {
            using (UserContext userContext = new UserContext())
            {
                try
                {
                    List<MessageT> searchingResult = new List<MessageT>();
                    foreach (MessageT message in userContext.Messages.Where(mes => mes.Type == "TEXT").ToList())
                    {
                        string textMessage = System.Text.Encoding.UTF8.GetString(message.Content);
                        if (textMessage.Contains(keyWord))
                        {
                            searchingResult.Add(message);
                        }
                    }
                    return searchingResult;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public ApplicationSettings getAppSettings(User user)
        {
            using (UserContext context =  new UserContext())
            {
                ApplicationSettings appSettings = context.ApplicationSettings.Where(set => set.UserId == user.Id).FirstOrDefault();
                return appSettings;
            }
        }

        public bool saveAppSettings(ApplicationSettings appSettings)
        {
            using (UserContext context = new UserContext())
            {
                try
                {
                    ApplicationSettings appSettingsDb = context.ApplicationSettings.Where(set => set.UserId == appSettings.UserId).FirstOrDefault();

                    if (appSettingsDb != null)
                    {
                        appSettingsDb.Language = appSettings.Language;
                        appSettingsDb.Theme = appSettings.Theme;
                        appSettingsDb.UserId = appSettings.UserId;
                        appSettingsDb.WindowSize = appSettings.WindowSize;
                        appSettingsDb.AllowNotifications = appSettings.AllowNotifications;
                    }
                    else
                    {
                        context.ApplicationSettings.Add(appSettings);
                    }

                    context.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}
