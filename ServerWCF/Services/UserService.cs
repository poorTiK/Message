using Message.AdditionalItems;
using ServerWCF.Context;
using ServerWCF.Contracts;
using ServerWCF.Model;
using ServerWCF.Model.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Threading;

namespace ServerWCF.Services
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class UserService : IUserService
    {
        private static readonly string successResult = "";

        private static readonly string basePicturePath = @"../../resources/BaseAvatar.jpg";

        private static List<CallbackData> usersOnline = new List<CallbackData>();

        private class MessageInfo
        {
            public BaseMessage Message { get; set; }
            public CallbackData CallbackData { get; set; }
        }

        private class CallbackData
        {
            public User User { get; set; }
            public IUserCallback UserCallback { get; set; }
        }

        //contacts
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

        //users
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

        public string AddOrUpdateUser(User user)
        {
            string validationInfo = Validate(user);
            if (validationInfo != successResult)
            {
                return validationInfo;
            }

            using (UserContext userContext = new UserContext())
            {
                try
                {
                    User dbUser = userContext.Users.Where(u => u.Login == user.Login).FirstOrDefault();

                    if (dbUser != null)
                    {
                        dbUser.Email = user.Email;
                        dbUser.Bio = user.Bio;
                        dbUser.Avatar = user.Avatar;
                        dbUser.FirstName = user.FirstName;
                        dbUser.Status = user.Status;
                        dbUser.Phone = user.Phone;
                        dbUser.Password = user.Password;
                    }
                    else
                    {
                        userContext.Users.Add(user);
                    }

                    userContext.SaveChanges();

                    return successResult;
                }
                catch (Exception ex)
                {
                    return "Exceptions occured during adding user.";
                }
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

        public User GetUser(string login, byte[] password)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    foreach (var user in db.Users)
                    {
                        if (user.Login == login && user.Password.SequenceEqual(password))
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
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public List<User> FindUsersByLogin(string keyWorkForLogin)
        {
            using (UserContext usersContext = new UserContext())
            {
                List<User> searchinfResult = new List<User>();
                try
                {
                    searchinfResult = usersContext.Users.Where(u => u.Login.Contains(keyWorkForLogin)).ToList();
                }
                catch (Exception ex)
                {
                    searchinfResult = new List<User>();
                }

                return searchinfResult;
            }
        }

        //application settings
        public ApplicationSettings GetAppSettings(User user)
        {
            using (UserContext context = new UserContext())
            {
                ApplicationSettings appSettings = context.ApplicationSettings.Where(set => set.UserId == user.Id).FirstOrDefault();
                return appSettings;
            }
        }

        public bool SaveAppSettings(ApplicationSettings appSettings)
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

        //online callbacks
        public void OnUserCame(User user)
        {
            using (UserContext userContext = new UserContext())
            {
                User dbUser = userContext.Users.Where(u => u.Id == user.Id).FirstOrDefault();
                dbUser.Status = "online";
                userContext.SaveChanges();
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

        public void OnUserLeave(User user)
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

        private void userLeaveCallback(object callbackDataObj)
        {
            CallbackData callbackData = callbackDataObj as CallbackData;

            foreach (CallbackData innerCallbackData in usersOnline)
            {
                innerCallbackData.UserCallback.UserLeave(callbackData.User);
            }
        }

        //messages
        public List<GroupMessage> GetGroupMessages(ChatGroup group, int limit)
        {
            using (UserContext context = new UserContext())
            {
                List<GroupMessage> messagesToReturn = new List<GroupMessage>();
                try
                {
                    foreach (BaseMessage message in context.Messages)
                    {
                        if (message is GroupMessage)
                        {
                            GroupMessage groupMessage = message as GroupMessage;
                            if (messagesToReturn.Count == limit)
                            {
                                break;
                            }
                            if (group.Id == groupMessage.ChatGroupId)
                            {
                                messagesToReturn.Add(groupMessage);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    messagesToReturn = new List<GroupMessage>();
                }

                return messagesToReturn;
            }
        }

        public List<UserMessage> GetUserMessages(User sender, User receiver, int limin)
        {
            using (UserContext context = new UserContext())
            {
                List<UserMessage> messagesToReturn = new List<UserMessage>();
                try
                {
                    foreach (BaseMessage message in context.Messages)
                    {
                        if (message is UserMessage)
                        {
                            UserMessage userMessage = message as UserMessage;
                            if (messagesToReturn.Count == limin)
                            {
                                break;
                            }

                            if (userMessage.SenderId == sender.Id &&
                                userMessage.ReceiverId == receiver.Id)
                            {
                                messagesToReturn.Add(userMessage);
                            }

                            if (userMessage.SenderId == receiver.Id &&
                                userMessage.ReceiverId == sender.Id)
                            {
                                messagesToReturn.Add(userMessage);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    messagesToReturn = new List<UserMessage>();
                }

                return messagesToReturn;
            }
        }

        public List<BaseMessage> FindMessage(string keyWord)
        {
            using (UserContext userContext = new UserContext())
            {
                List<BaseMessage> searchingResult = new List<BaseMessage>();

                try
                {
                    foreach (BaseMessage message in userContext.Messages.Where(mes => mes.Type == "TEXT").ToList())
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
                    searchingResult = new List<BaseMessage>();
                }

                return searchingResult;
            }
        }

        public void EditMessage(BaseMessage editedMessage)
        {
            string validationInfo = Validate(editedMessage);
            if (validationInfo != successResult)
            {
                return;
            }

            using (UserContext userContext = new UserContext())
            {
                try
                {
                    BaseMessage dbMessage = userContext.Messages.Where(mes => mes.Id == editedMessage.Id).FirstOrDefault();
                    if (dbMessage == null)
                    {
                        return;
                    }

                    if (dbMessage is UserMessage)
                    {
                        UserMessage userMessage = dbMessage as UserMessage;
                        User dbReceiver = userContext.Users.Where(u => u.Id == userMessage.ReceiverId).FirstOrDefault();
                        userMessage.Receiver = dbReceiver;
                        userMessage.ReceiverId = dbReceiver.Id;
                    }
                    else if (dbMessage is GroupMessage)
                    {
                        GroupMessage groupMessage = dbMessage as GroupMessage;
                        ChatGroup chatGroup = userContext.ChatGroups.Where(g => g.Id == groupMessage.ChatGroupId).FirstOrDefault();
                        groupMessage.ChatGroup = chatGroup;
                        groupMessage.ChatGroupId = chatGroup.Id;
                    }
                    dbMessage.Sender = editedMessage.Sender;
                    dbMessage.SenderId = editedMessage.SenderId;
                    dbMessage.Type = editedMessage.Type;
                    dbMessage.DateOfSending = editedMessage.DateOfSending;
                    dbMessage.Content = editedMessage.Content;

                    CallbackData callbackData = usersOnline.Where(cd => cd.User.Id == dbMessage.SenderId).FirstOrDefault();
                    if (callbackData == null)
                    {
                        return;
                    }

                    userContext.SaveChanges();

                    MessageInfo messageInfo = new MessageInfo();
                    messageInfo.Message = dbMessage;
                    messageInfo.CallbackData = callbackData;

                    Thread t = new Thread(new ParameterizedThreadStart(EditMessageCallback));
                    t.IsBackground = true;
                    t.Start(messageInfo);
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        private void EditMessageCallback(object mesDataObj)
        {
            MessageInfo messageInfo = mesDataObj as MessageInfo;

            foreach (CallbackData innerCallbackData in usersOnline)
            {
                if (innerCallbackData != messageInfo.CallbackData)
                {
                    innerCallbackData.UserCallback.OnMessageEdited(messageInfo.Message);
                }
            }
        }

        public void RemoveMessage(BaseMessage removedMessage)
        {
            using (UserContext userContext = new UserContext())
            {
                try
                {
                    BaseMessage dbBaseMessage = userContext.Messages.Where(mes => mes.Id == removedMessage.Id).FirstOrDefault();
                    if (dbBaseMessage == null)
                    {
                        return;
                    }

                    userContext.Messages.Remove(dbBaseMessage);

                    CallbackData callbackData = usersOnline.Where(cd => cd.User.Id == removedMessage.SenderId).FirstOrDefault();
                    if (callbackData == null)
                    {
                        return;
                    }

                    userContext.SaveChanges();

                    MessageInfo messageInfo = new MessageInfo();
                    messageInfo.Message = dbBaseMessage;
                    messageInfo.CallbackData = callbackData;

                    Thread t = new Thread(new ParameterizedThreadStart(RemoveMessageCallback));
                    t.IsBackground = true;
                    t.Start(messageInfo);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void RemoveMessageCallback(object mesDataObj)
        {
            MessageInfo messageInfo = mesDataObj as MessageInfo;

            foreach (CallbackData innerCallbackData in usersOnline)
            {
                if (innerCallbackData != messageInfo.CallbackData)
                {
                    innerCallbackData.UserCallback.OnMessageRemoved(messageInfo.Message);
                }
            }
        }

        public void SendMessage(BaseMessage message)
        {
            string validationInfo = Validate(message);
            if (validationInfo != successResult)
            {
                return;
            }

            using (UserContext userContext = new UserContext())
            {
                try
                {
                    User dbSender = userContext.Users.Where(u => u.Id == message.SenderId).First();
                    message.Sender = dbSender;

                    CallbackData callbackData = usersOnline.Where(cd => cd.User.Id == dbSender.Id).FirstOrDefault();
                    if (callbackData == null)
                    {
                        return;
                    }

                    if (message is UserMessage)
                    {
                        UserMessage userMessage = message as UserMessage;
                        User dbReceiver = userContext.Users.Where(u => u.Id == userMessage.ReceiverId).FirstOrDefault();
                        userMessage.Receiver = dbReceiver;
                    }
                    else if (message is GroupMessage)
                    {
                        GroupMessage groupMessage = message as GroupMessage;
                        ChatGroup chatGroup = userContext.ChatGroups.Where(g => g.Id == groupMessage.ChatGroupId).FirstOrDefault();
                        groupMessage.ChatGroup = chatGroup;
                    }

                    userContext.Messages.Add(message);
                    userContext.SaveChanges();

                    MessageInfo messageInfo = new MessageInfo();
                    messageInfo.Message = message;
                    messageInfo.CallbackData = callbackData;

                    Thread t = new Thread(new ParameterizedThreadStart(ReceiveMessageCallback));
                    t.IsBackground = true;
                    t.Start(messageInfo);
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        private void ReceiveMessageCallback(object mesDataObj)
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

        //validation
        private bool IsEmailUnique(List<User> users, string email)
        {
            using(UserContext userContext = new UserContext())
            {
                return userContext.Users.Where(u => u.Email == email).FirstOrDefault() == null;
            }
        }

        private bool IsLoginUnique(List<User> users, string login)
        {
            using (UserContext userContext = new UserContext())
            {
                return userContext.Users.Where(u => u.Login == login).FirstOrDefault() == null;
            }
        }

        private string Validate(User user)
        {
            using (UserContext userContext = new UserContext())
            {
                if (!IsEmailUnique(userContext.Users.ToList(), user.Email))
                {
                    return "Email is not unique.";
                }
            }
            return successResult;
        }

        private string Validate(BaseMessage baseMessage)
        {
            if (baseMessage.Content.Length == 0)
            {
                return "Message is empty.";
            }

            if (baseMessage.SenderId == 0)
            {
                return "Sender is not specified.";
            }

            if (baseMessage is GroupMessage)
            {
                GroupMessage groupMessage = baseMessage as GroupMessage;
                if (groupMessage.ChatGroupId == 0)
                {
                    return "Chat group is not specified.";
                }
            }
            else if (baseMessage is UserMessage)
            {
                UserMessage userMessage = baseMessage as UserMessage;
                if (userMessage.ReceiverId == 0)
                {
                    return "Receiver is not specified.";
                }
            }

            return successResult;
        }

        //private string ValidateAddingContact(User owner, User owned)
        //{
        //    using (UserContext userContext = new UserContext())
        //    {
        //        User userOwner = userContext.Users.Include("Contacts").Where(u => u.Id == owner.Id).FirstOrDefault();

                
        //    }
        //}
    }
}
