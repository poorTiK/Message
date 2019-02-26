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
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class UserService : IUserService
    {
        private static List<CallbackData> usersOnline = new List<CallbackData>();

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

        public bool IsExistsInContacts(User owner, User owned)
        {
            List<User> contactsForOwner = GetAllContacts(owner);

            return contactsForOwner.Where(u => u.Id == owned.Id).FirstOrDefault() != null;
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
                        result.Status = user.Status;
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

        public List<User> GetAllContacts(User owner)
        {
            using (UserContext db = new UserContext())
            {
                List<User> contactsForOwner = new List<User>();
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
                    contactsForOwner = new List<User>();
                }

                return contactsForOwner;
            }
            
        }

        public List<User> GetAllUsers()
        {
            using (UserContext db = new UserContext())
            {
                List<User> allUsers = new List<User>();
                try
                {
                    allUsers = db.Users.ToList();
                }
                catch (Exception ex)
                {
                    allUsers = new List<User>();
                }
                return allUsers;
            }
        }

        public User GetUserByLogin(string login)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    return db.Users.Where(x => x.Login.Contains(login)).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    return null;
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
                List<MessageT> messagesToReturn = new List<MessageT>();
                try
                {
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
                }
                catch (Exception ex)
                {
                    messagesToReturn = new List<MessageT>();
                }

                return messagesToReturn;
            }
        }

        public List<MessageT> FindMessage(string keyWord)
        {
            using (UserContext userContext = new UserContext())
            {
                List<MessageT> searchingResult = new List<MessageT>();

                try
                {
                    foreach (MessageT message in userContext.Messages.Where(mes => mes.Type == "TEXT").ToList())
                    {
                        string textMessage = System.Text.Encoding.UTF8.GetString(message.Content);
                        if (textMessage.Contains(keyWord))
                        {
                            searchingResult.Add(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    searchingResult = new List<MessageT>();
                }

                return searchingResult;
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

        public void onUserCame(User user)
        {
            using(UserContext userContext = new UserContext())
            {
                User dbUser = userContext.Users.Where(u => u.Id == user.Id).FirstOrDefault();

                if (dbUser != null)
                {
                    CallbackData callbackData = new CallbackData();
                    callbackData.User = dbUser;

                    IUserCallback callback = OperationContext.Current.GetCallbackChannel<IUserCallback>();
                    callbackData.UserCallback = callback;

                    usersOnline.Add(callbackData);

                    Thread t = new Thread(new ParameterizedThreadStart(userCameCallback));
                    t.IsBackground = true;
                    t.Start(callbackData);
                }
            }
        }

        public void onUserLeave(User user)
        {
            using (UserContext userContext = new UserContext())
            {
                CallbackData callbackData = usersOnline.Where(cd => cd.User.Id == user.Id).FirstOrDefault();
                if (callbackData != null)
                {
                    usersOnline.Remove(callbackData);

                    User userToChangeStatus = userContext.Users.Where(u => u.Id == user.Id).FirstOrDefault();
                    userToChangeStatus.Status = DateTime.Now.ToString();
                    userContext.SaveChanges();

                    foreach (CallbackData innerCallbackData in usersOnline)
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(userLeaveCallback));
                        t.IsBackground = true;
                        t.Start(innerCallbackData);
                    }
                }
            }
        }

        private void userCameCallback(object callbackDataObj)
        {
            CallbackData callbackData = callbackDataObj as CallbackData;

            foreach (CallbackData innerCallbackData in usersOnline)
            {
                if (innerCallbackData != callbackData)
                {
                    innerCallbackData.UserCallback.UserCame(callbackData.User);
                }
            }
        }

        private void userLeaveCallback(object callbackDataObj)
        {
            CallbackData callbackData = callbackDataObj as CallbackData;

            foreach(CallbackData innerCallbackData in usersOnline)
            {
                innerCallbackData.UserCallback.UserLeave(callbackData.User);
            }
        }

        public List<User> FindUsersByLogin(string keyWorkForLogin)
        {
            using(UserContext usersContext = new UserContext()) 
            {
                List<User> searchinfResult = new List<User>();
                try
                {
                    searchinfResult = usersContext.Users.Where(u => u.Login.Contains(keyWorkForLogin)).ToList();
                }
                catch(Exception ex)
                {
                    searchinfResult = new List<User>();
                }

                return searchinfResult;
            } 
        }

        public void SendMessage(MessageT message)
        {
            using (UserContext userContext = new UserContext())
            {
                try
                {
                    User dbSender = userContext.Users.Where(u => u.Id == message.SenderId).First();

                    CallbackData callbackData = usersOnline.Where(cd => cd.User.Id == dbSender.Id).FirstOrDefault();
                    if (callbackData == null)
                    {
                        return;
                    }

                    User dbReceiver = userContext.Users.Where(u => u.Id == message.ReceiverId).First();

                    message.Sender = dbSender;
                    message.Receiver = dbReceiver;

                    userContext.Messages.Add(message);
                    userContext.SaveChanges();

                    MessageInfo messageInfo = new MessageInfo();
                    messageInfo.Message = message;
                    messageInfo.CallbackData = callbackData;

                    Thread t = new Thread(new ParameterizedThreadStart(receiveMessageCallback));
                    t.IsBackground = true;
                    t.Start(messageInfo);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void receiveMessageCallback(object mesDataObj)
        {
            MessageInfo messageInfo = mesDataObj as MessageInfo;

            foreach (CallbackData innerCallbackData in usersOnline)
            {
                if (innerCallbackData != messageInfo.CallbackData)
                {
                    innerCallbackData.UserCallback.ReceiveMessage(messageInfo.Message);
                }
            }
        }

        private class MessageInfo
        {
            public MessageT Message { get; set; }
            public CallbackData CallbackData { get; set; }
        }

        private class CallbackData
        {
            public User User
            {
                get;set;
            }
            public IUserCallback UserCallback
            {
                get;set;
            }
        }
    }
}
