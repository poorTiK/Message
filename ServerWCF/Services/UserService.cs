﻿using ServerWCF.Context;
using ServerWCF.Contracts;
using ServerWCF.Model;
using ServerWCF.Model.Contacts;
using ServerWCF.Model.Messages;
using ServerWCF.Model.UiInfo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace ServerWCF.Services
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public class UserService : IUserService
    {
        private static readonly string successResult = "";

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
        public bool AddUserToUserContact(int id_owner, int id_owned)
        {
            using (UserContext userContext = new UserContext())
            {
                try
                {
                    UserToUserContact contact = new UserToUserContact();

                    User ownerFromDb = userContext.Users.FirstOrDefault(dbUser => dbUser.Id == id_owner);
                    User ownedFromDb = userContext.Users.FirstOrDefault(dbUser => dbUser.Id == id_owned);

                    contact.UserOwner = ownerFromDb;
                    contact.UserOwned = ownedFromDb;

                    if (userContext.Contacts.FirstOrDefault(c => ( (c.UserOwnerId == id_owner) && ( (c as UserToUserContact).UserOwnedId == id_owned) )) != null )
                    {
                        return false;
                    }

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

        public bool AddUserToChatGroupContact(int chatGroupId, int participantId)
        {
            using (UserContext userContext = new UserContext())
            {
                try
                {
                    UserToGroupContact contact = new UserToGroupContact();

                    ChatGroup dbChatGroup = userContext.ChatGroups.FirstOrDefault(chatGroup => chatGroup.Id == chatGroupId);
                    User ownedFromDb = userContext.Users.FirstOrDefault(dbUser => dbUser.Id == participantId);

                    contact.ChatGroup = dbChatGroup;
                    contact.UserOwner = ownedFromDb;

                    if (userContext.Contacts.FirstOrDefault(c => ((c.UserOwnerId == participantId) && ((c as UserToGroupContact).ChatGroupId == chatGroupId))) != null)
                    {
                        return false;
                    }

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

        public bool RemoveUserToUserContact(int ownerId, int ownedId)
        {
            using (UserContext userContext = new UserContext())
            {
                try
                {
                    List<BaseContact> contacts = userContext.Contacts.Include("UserOwner").ToList();

                    User ownerFromDb = userContext.Users.Where(u => u.Id == ownerId).FirstOrDefault();
                    User ownedFromDb = userContext.Users.Where(u => u.Id == ownedId).FirstOrDefault();

                    foreach (BaseContact contact in contacts)
                    {
                        if (contact.UserOwnerId == ownerFromDb.Id && (contact as UserToUserContact).UserOwnedId == ownedFromDb.Id)
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

        public bool RemoveUserToChatGroupContact(int chatGroupId, int participantId)
        {
            using (UserContext userContext = new UserContext())
            {
                try
                {
                    List<BaseContact> contacts = userContext.Contacts.Include("UserOwner").ToList();

                    ChatGroup ownerFromDb = userContext.ChatGroups.Where(cg => cg.Id == chatGroupId).FirstOrDefault();
                    User ownedFromDb = userContext.Users.Where(u => u.Id == participantId).FirstOrDefault();

                    foreach (BaseContact contact in contacts)
                    {
                        if (contact.UserOwnerId == ownerFromDb.Id && (contact as UserToGroupContact).ChatGroupId == chatGroupId)
                        {
                            userContext.Contacts.Remove(contact);
                            userContext.SaveChanges();
                            return true;
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
        }

        public bool IsExistsInContacts(int id_owner, int id_owned)
        {
            List<User> contactsForOwner = GetAllUsersContacts(id_owner);

            return contactsForOwner.FirstOrDefault(u => u.Id == id_owned) != null;
        }

        public List<User> GetAllUsersContacts(int userId)
        {
            using (UserContext db = new UserContext())
            {
                List<User> contactsForOwner = new List<User>();
                try
                {
                    contactsForOwner = db.Users.SqlQuery(" select * " +
                            "from Users " +
                            "where Users.Id in (select UserOwnedId " +
                            "from BaseContacts " +
                            "where BaseContacts.UserOwnerId = @p0);", userId).ToList();
                }
                catch (Exception ex)
                {
                    contactsForOwner = new List<User>();
                }
                return contactsForOwner;
            }
        }

        public List<ChatGroup> GetAllChatGroupsContacts(int userId)
        {
            using (UserContext db = new UserContext())
            {
                List<ChatGroup> contactsForOwner = new List<ChatGroup>();
                try
                {
                    contactsForOwner = db.ChatGroups.SqlQuery("select * " +
                          " from ChatGroups" +
                          " where ChatGroups.Id in (select ChatGroupId" +
                          " from BaseContacts" +
                          " where BaseContacts.UserOwnerId = @p0);", userId).ToList();
                }
                catch (Exception ex)
                {
                    contactsForOwner = new List<ChatGroup>();
                }
                return contactsForOwner;
            }
        }

        public List<UiInfo> GetAllContactsUiInfo(int id)
        {
            List<UiInfo> usersUiInfos = new List<UiInfo>(GetAllUsersContacts(id).Select( u => new UserUiInfo(u)));
            List<UiInfo> chatGroupsUiInfo = new List<UiInfo>(GetAllChatGroupsContacts(id).Select( cg => new ChatGroupUiInfo(cg) ));

            usersUiInfos.AddRange(chatGroupsUiInfo);

            return usersUiInfos;
        }

        //chatGroups
        public string AddOrUpdateChatGroup(ChatGroup chatGroupToAdd)
        {
            using (UserContext userContext = new UserContext())
            {
                try
                {
                    ChatGroup dbChatGroup = userContext.ChatGroups.FirstOrDefault(cg => cg.Name == chatGroupToAdd.Name);

                    if (dbChatGroup != null)
                    {
                        dbChatGroup.Participants = chatGroupToAdd.Participants;
                    }
                    else
                    {
                        string validationInfo = Validate(chatGroupToAdd);
                        if (validationInfo != successResult)
                        {
                            return validationInfo;
                        }

                        userContext.ChatGroups.Add(chatGroupToAdd);
                        userContext.SaveChanges();
                    }

                    return successResult;
                }
                catch (Exception ex)
                {
                    return "Exceptions were occured during adding new chat group.";
                }
            }
        }

        public ChatGroup GetChatGroup(string chatGroupName)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    return db.ChatGroups.FirstOrDefault(cg => cg.Name.Contains(chatGroupName));
                }
                catch (Exception ex)
                {
                    return null;
                }
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

        public List<UiInfo> GetAllUsersUiInfo()
        {
            List<User> allUsers = GetAllUsers();
            List<UiInfo> uiInfoToReturn = new List<UiInfo>(allUsers.Select(u => new UserUiInfo(u)).ToList());

            return uiInfoToReturn;
        }

        public string AddOrUpdateUser(User user)
        {
            using (UserContext userContext = new UserContext())
            {
                try
                {
                    User dbUser = userContext.Users.Where(u => u.Login == user.Login).FirstOrDefault();

                    if (dbUser != null)
                    {

                        dbUser.Email = user.Email;
                        dbUser.Bio = user.Bio;
                        dbUser.FirstName = user.FirstName;
                        dbUser.Status = user.Status;
                        dbUser.Phone = user.Phone;
                        dbUser.Password = user.Password;
                    }
                    else
                    {
                        string validationInfo = Validate(user);
                        if (validationInfo != successResult)
                        {
                            return validationInfo;
                        }

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
                   User user =  db.Users.Where(x => x.Login.Contains(login)).FirstOrDefault();
                   return user;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public User GetUser(string login, byte[] password)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Login == login);
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public User GetUserById(int id)
        {
            try
            {
                using (UserContext db = new UserContext())
                {
                    User user = db.Users.FirstOrDefault(u => u.Id == id);
                    return user;
                }
            }
            catch (Exception)
            {
            }

            return null;
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
                        {
                            return user;
                        }
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

        public List<UiInfo> FindUsersUiUnfoByLogin(string keyWorkForLogin)
        {
            List<UiInfo> usersUiInfos = new List<UiInfo>(FindUsersByLogin(keyWorkForLogin).Select(u => new UserUiInfo(u)));
            return usersUiInfos;
        }

        //application settings
        public ApplicationSettings GetAppSettings(int userId)
        {
            using (UserContext context = new UserContext())
            {
                ApplicationSettings appSettings = context.ApplicationSettings.Where(set => set.UserId == userId).FirstOrDefault();
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
        public void OnUserCame(int userId)
        {
            using (UserContext userContext = new UserContext())
            {
                User dbUser = userContext.Users.Where(u => u.Id == userId).FirstOrDefault();
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

        public void OnUserLeave(int userId)
        {
            using (UserContext userContext = new UserContext())
            {
                CallbackData callbackData = usersOnline.Where(cd => cd.User.Id == userId).FirstOrDefault();
                if (callbackData != null)
                {
                    usersOnline.Remove(callbackData);

                    User userToChangeStatus = userContext.Users.Where(u => u.Id == userId).FirstOrDefault();
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
        public List<GroupMessage> GetGroupMessages(int chatGroupId, int limit)
        {
            using (UserContext context = new UserContext())
            {
                List<GroupMessage> messagesToReturn = new List<GroupMessage>();
                try
                {
                    foreach (BaseMessage message in context.Messages.AsEnumerable().Reverse())
                    {
                        if (message is GroupMessage)
                        {
                            GroupMessage groupMessage = message as GroupMessage;
                            if (messagesToReturn.Count == limit)
                            {
                                break;
                            }
                            if (chatGroupId == groupMessage.ChatGroupId)
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

                messagesToReturn.Reverse();
                return messagesToReturn;
            }
        }

        public List<UserMessage> GetUserMessages(int sender, int receiver, int limin)
        {
            using (UserContext context = new UserContext())
            {
                List<UserMessage> messagesToReturn = new List<UserMessage>();
                try
                {
                    foreach (BaseMessage message in context.Messages.AsEnumerable().Reverse())
                    {
                        if (message is UserMessage)
                        {
                            UserMessage userMessage = message as UserMessage;

                            if (messagesToReturn.Count == limin)
                            {
                                break;
                            }

                            if (userMessage.SenderId == sender &&
                                userMessage.ReceiverId == receiver)
                            {
                                messagesToReturn.Add(userMessage);
                            }

                            if (userMessage.SenderId == receiver &&
                                userMessage.ReceiverId == sender)
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
                messagesToReturn.Reverse();
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
                    foreach (BaseMessage message in userContext.Messages)
                    {
                        string textMessage = System.Text.Encoding.UTF8.GetString(message.Text);
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
                    dbMessage.DateOfSending = editedMessage.DateOfSending;

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
                    User dbSender = userContext.Users.Where(u => u.Id == message.SenderId).FirstOrDefault();
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

                    //if (message.Type == "DATA")
                    //{
                    //    var serv = new PhotoService();

                    //    serv.SetFileToMessage(userContext.Messages.Last().Id, message.Content);
                    //}

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

        private string Validate(ChatGroup chatGroupToAdd)
        {
            if (chatGroupToAdd.Name.Length == 0)
            {
                return "Message is empty.";
            }

            return successResult;
        }

        private string Validate(BaseMessage baseMessage)
        {
            if (baseMessage.Text.Length == 0)
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
    }
}
