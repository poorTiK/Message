using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Message.PhotoServiceReference;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class MessageMainVM : BaseViewModel
    {

        private IMessaging _view;
        private Image _image;

        public Image Images
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Images"));
            }
        }


        public User CurrentUser { get; set; }
       //private byte[] _currentUserPhoto;

       // public byte[] CurrentUserPhoto
       // {
       //     get { return GlobalBase.CurrentUser.Avatar; }
       //     set { SetProperty(ref _currentUserPhoto, value); }
       // }
        private string _searchContactStr;

        public string SearchContactStr 
        {
            get { return _searchContactStr; }
            set
            {
                if (value == string.Empty)
                {
                    ContactsList.Clear();
                    ContactsList.AddRange(UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id));

                    foreach (var item in ContactsList)
                    {
                        if (item is UserUiInfo)
                        {
                            UserUiInfo userUiInfo = item as UserUiInfo;
                            item.Avatar = GlobalBase.PhotoServiceClient.GetPhotoById(userUiInfo.UserId);
                        }
                        else if (item is ChatGroupUiInfo)
                        {
                            //todo: make ability to get picture for group
                        }
                    }
                }
                else
                {
                    ContactsList.Clear();
                    ContactsList.AddRange(UserServiceClient
                        .GetAllUsersContacts(GlobalBase.CurrentUser.Id)
                        .Where(i => i.FirstName.Contains(value)
                        || i.LastName.Contains(value)
                        || i.Login.Contains(value))
                                  .Select(u => new UserUiInfo
                                  {
                                      Name = u.FirstName + " " + u.LastName,
                                      UniqueName = u.Login,
                                      UserId = u.Id,
                                      Avatar = u.Avatar,
                                      Status = u.Status
                                  }));


                    foreach (var item in ContactsList)
                    {
                        if (item is UserUiInfo)
                        {
                            UserUiInfo userUiInfo = item as UserUiInfo;
                            item.Avatar = GlobalBase.PhotoServiceClient.GetPhotoById(userUiInfo.UserId);
                        }
                        else if (item is ChatGroupUiInfo)
                        {
                            //todo: make ability to get picture for group
                        }
                    }
                }

                SetProperty(ref _searchContactStr, value);
            }
        }
        private string _currentUserName;

        public string CurrentUserName
        {
            get { return CurrentUser.FirstName + " " + CurrentUser.LastName; }
            set { SetProperty(ref _currentUserName, value); }
        }

        private string _currentUserLogin;

        public string CurrentUserLogin
        {
            get { return "@" + CurrentUser.Login; }
            set { SetProperty(ref _currentUserLogin, value); }
        }

        private bool _isDialogSearchVisible;

        public bool IsDialogSearchVisible
        {
            get { return _isDialogSearchVisible; }
            set { SetProperty(ref _isDialogSearchVisible, value); }
        }

        private List<UiInfo> _contactsList;

        public List<UiInfo> ContactsList
        {
            get { return _contactsList; }
            set { SetProperty(ref _contactsList, value); }
        }

        private UiInfo _selectedContact;

        public UiInfo SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                SetProperty(ref _selectedContact, value, () => {SelectedContactChanged();});
            }
        }

        private string _dialogSearchStr;

        public string DialogSearchStr
        {
            get { return _dialogSearchStr; }
            set
            {
                SetProperty(ref _dialogSearchStr, value);
                OnDialogSearch();
            }
        }

        private string _messageText;

        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); }
        }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { SetProperty(ref _filePath, value); }
        }

        private bool _isMenuEnabled;

        public bool IsMenuEnabled
        {
            get { return _isMenuEnabled; }
            set { SetProperty(ref _isMenuEnabled, value); }
        }

        public MessageMainVM(IMessaging View, User user) : base()
        {
            _view = View;

            CurrentUser = user;
            GlobalBase.CurrentUser = user;
            GlobalBase.UpdateUI += () =>
            {
                Update();
            };

            SetAvatarForUI();

            ContactsList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);
            foreach (var item in ContactsList)
            {
                if (item.Avatar != null)
                {
                    MemoryStream memstr = new MemoryStream(item.Avatar);
                    Dispatcher.CurrentDispatcher.Invoke(() => { item.Images = Image.FromStream(memstr); });
                    ContactsList = ContactsList.ToList();
                }
            }
            IsMenuEnabled = false;
            SelectedContact = ContactsList.FirstOrDefault();

            UserServiceClient.OnUserCame(user.Id);
        }

        private void SelectedContactChanged(object sender = null, PropertyChangedEventArgs e = null)
        {
            if (SelectedContact != null)
            { IsMenuEnabled = true; }
            else
            { IsMenuEnabled = false;}

            if (_view.MessageList != null)
            {
                _view.MessageList.Clear();

                List<BaseMessage> res = new List<BaseMessage>();
                if (SelectedContact is UserUiInfo)
                {
                    res.AddRange(UserServiceClient.GetUserMessages(GlobalBase.CurrentUser.Id, (SelectedContact as UserUiInfo).UserId, 50));
                }
                else if (SelectedContact is ChatGroupUiInfo)
                {
                    res.AddRange(UserServiceClient.GetGroupMessages((SelectedContact as ChatGroupUiInfo).ChatGroupId, 50));
                }

                if (res.Count != 0)
                {
                    foreach (BaseMessage mes in res)
                    {
                        if (mes is UserMessage)
                        {
                            UserMessage userMessage = mes as UserMessage;
                            _view.MessageList.Add(new UserMessage()
                            {
                                Id = mes.Id,
                                Content = mes.Content,
                                DateOfSending = mes.DateOfSending,
                                ReceiverId = userMessage.ReceiverId,
                                SenderId = mes.SenderId,
                                Type = mes.Type
                            });
                        }
                        else if (mes is GroupMessage)
                        {
                            GroupMessage chatGroupMessage = mes as GroupMessage;
                            _view.MessageList.Add(new GroupMessage()
                            {
                                Id = mes.Id,
                                Content = mes.Content,
                                DateOfSending = mes.DateOfSending,
                                ChatGroupId = chatGroupMessage.ChatGroupId,
                                SenderId = mes.SenderId,
                                Type = mes.Type
                            });
                        }
                        
                    }
                    _view.UpdateMessageList();
                }

            }
        }

        private DelegateCommand _onContactsCommand;

        public DelegateCommand ContactsCommand =>
            _onContactsCommand ?? (_onContactsCommand = new DelegateCommand(ExecuteOnContacts));

        private DelegateCommand _onSettingsCommand;

        public DelegateCommand SettingsCommand =>
            _onSettingsCommand ?? (_onSettingsCommand = new DelegateCommand(ExecuteOnSettingsCommand));

        private DelegateCommand _dialogSearchCommand;

        public DelegateCommand DialogSearchCommand =>
            _dialogSearchCommand ?? (_dialogSearchCommand = new DelegateCommand(() =>
            {
                IsDialogSearchVisible = !IsDialogSearchVisible;
                if (!IsDialogSearchVisible)
                {
                    SelectedContactChanged();
                }
            }));

        private DelegateCommand _onSendMessage;

        public DelegateCommand OnSendMessage =>
            _onSendMessage ?? (_onSendMessage = new DelegateCommand(ExecuteOnSendMessage));

        private DelegateCommand _onViewProfile;

        public DelegateCommand ViewProfile =>
            _onViewProfile ?? (_onViewProfile = new DelegateCommand(ExecuteOnViewProfile));

        private DelegateCommand _exit;

        public DelegateCommand Exit =>
            _exit ?? (_exit = new DelegateCommand(ExecuteOnExit));

        private DelegateCommand _addFile;

        public DelegateCommand AddFile =>
            _addFile ?? (_addFile = new DelegateCommand(ExecuteOnAddFile));

        private void ExecuteOnAddFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            FilePath = openFileDialog.FileName;
        }

        private void ExecuteOnExit()
        {
           UserServiceClient.OnUserLeave(GlobalBase.CurrentUser.Id);
            _view.CloseWindow();
        }

        private void ExecuteOnViewProfile()
        {
            if (SelectedContact is UserUiInfo)
            {
                UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                User user = UserServiceClient.GetUserById(userUiInfo.UserId);
                var wnd = new ContactProfileWindow(user);
                wnd.Owner = (Window)_view;
                wnd.ShowDialog();

                Update();
            }
        }

        private void ExecuteOnSendMessage()
        {
            if (SelectedContact != null && !string.IsNullOrWhiteSpace(MessageText))
            {
                BaseMessage message = null;
                if (SelectedContact is UserUiInfo)
                {
                    UserUiInfo userUiInfo = SelectedContact as UserUiInfo;

                    message = new UserMessage()
                    {
                        Content = Encoding.UTF8.GetBytes(MessageText),
                        DateOfSending = DateTime.Now,
                        SenderId = GlobalBase.CurrentUser.Id,
                        ReceiverId = userUiInfo.UserId,
                        Type = "TEXT",
                    };
                }
                else if (SelectedContact is ChatGroupUiInfo)
                {
                    ChatGroupUiInfo userUiInfo = SelectedContact as ChatGroupUiInfo;

                    message = new GroupMessage()
                    {
                        Content = Encoding.UTF8.GetBytes(MessageText),
                        DateOfSending = DateTime.Now,
                        SenderId = GlobalBase.CurrentUser.Id,
                        ChatGroupId = userUiInfo.ChatGroupId,
                        Type = "TEXT",
                    };
                }

                UserServiceClient.SendMessageAsync(message);

                Debug.WriteLine("Send Message");
                _view.MessageList.Add(message);
                _view.UpdateMessageList();

                MessageText = string.Empty;
            }
        }

        private void SetAvatarForUI()
        {
            if (GlobalBase.CurrentUser?.Avatar?.Length > 0)
            {
                MemoryStream memstr = new MemoryStream(GlobalBase.CurrentUser.Avatar);
                Dispatcher.CurrentDispatcher.Invoke(() => { Images = Image.FromStream(memstr); });
            }
            else
            {
                Dispatcher.CurrentDispatcher.Invoke(() => { Images = null; });
            }
        }

        private void ExecuteOnSettingsCommand()
        {
            _view.SetOpacity(0.5);

            var wnd = new SettingsWindow();
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();

            Update();

            _view.SetOpacity(1);
        }

        private void ExecuteOnContacts()
        {
            _view.SetOpacity(0.5);

            var wnd = new Contacts();
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();

            _view.SetOpacity(1);

            Update();
        }

        private void OnDialogSearch()
        {
            if (IsDialogSearchVisible)
            {
                if (SelectedContact != null)
                {
                    _view.MessageList.Clear();
                    List<BaseMessage> res = new List<BaseMessage>();

                    if (SelectedContact is UserUiInfo)
                    {
                        UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                        res.AddRange(UserServiceClient.GetUserMessages(GlobalBase.CurrentUser.Id, userUiInfo.UserId, 50));
                    }
                    else if (SelectedContact is ChatGroupUiInfo)
                    {
                        ChatGroupUiInfo chatGroupUiInfo = SelectedContact as ChatGroupUiInfo;
                        res.AddRange(UserServiceClient.GetGroupMessages(chatGroupUiInfo.ChatGroupId, 50));
                    }

                    if (res.Count != 0)
                    {
                        foreach (BaseMessage mes in res)
                        {
                            BaseMessage message = null;
                            if (mes is UserMessage)
                            {
                                UserMessage userMessage = mes as UserMessage;
                                message = new UserMessage()
                                {
                                    Content = mes.Content,
                                    DateOfSending = mes.DateOfSending,
                                    ReceiverId = userMessage.ReceiverId,
                                    SenderId = mes.SenderId,
                                    Type = mes.Type
                                };
                            }
                            else if (mes is GroupMessage)
                            {
                                GroupMessage groupMessage = mes as GroupMessage;
                                message = new GroupMessage()
                                {
                                    Content = mes.Content,
                                    DateOfSending = mes.DateOfSending,
                                    ChatGroupId = groupMessage.ChatGroupId,
                                    SenderId = mes.SenderId,
                                    Type = mes.Type
                                };
                            }

                            if (GlobalBase.Base64Decode(mes.Content).Contains(DialogSearchStr))
                            {
                                _view.MessageList.Add(message);
                            }
                        }
                        _view.UpdateMessageList();
                    }
                }
            }
            else
            {
                SelectedContactChanged();
            }
        }

        public void Update()
        {
            //make all update modular and put here plz
            UpdateContactList();
            SetAvatarForUI();
        }

        public void UpdateContactList()
        {
            ContactsList = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);

            foreach (var item in ContactsList)
            {
                if (SelectedContact is UserUiInfo)
                {
                    UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                    item.Avatar = GlobalBase.PhotoServiceClient.GetPhotoById(userUiInfo.UserId);
                }
                else if (SelectedContact is ChatGroupUiInfo)
                {
                    // todo: create ability to get picture for groups
                }
            }

            UiInfo temp = SelectedContact;

            if(temp is UserUiInfo)
            {
                UserUiInfo userUiInfo = temp as UserUiInfo;

                if (ContactsList.Any(x => temp != null && ((x as UserUiInfo).UserId == userUiInfo.UserId)))
                {
                    SelectedContact = temp;
                }
            } 
            else if(temp is ChatGroupUiInfo)
            {
                ChatGroupUiInfo chatGroupUiInfo = temp as ChatGroupUiInfo;

                if (ContactsList.Any(x => temp != null && ((x as ChatGroupUiInfo).ChatGroupId == chatGroupUiInfo.ChatGroupId)))
                {
                    SelectedContact = temp;
                }
            }


        }

        public override void ReceiveMessage(BaseMessage message)
        {
            User sender = UserServiceClient.GetAllUsers().FirstOrDefault(x => x.Id == message.SenderId);

            var mes = "New message from  @" + sender.Login + "\n" + "\"" + GlobalBase.Base64Decode(message.Content) +
                      "\"";
            GlobalBase.ShowNotify("New message", mes);

            Debug.WriteLine("Receave Message from - ", sender.Login);

            if (message is UserMessage)
            {
                if (ContactsList.Where(c => c is UserUiInfo).FirstOrDefault(x => (x as UserUiInfo).UserId == sender.Id) == null)
                {
                    UserServiceClient.AddUserToUserContactAsync(GlobalBase.CurrentUser.Id, sender.Id).ContinueWith(task =>
                    {
                        UpdateContactList();
                    });
                }
                else if ( (SelectedContact is UserUiInfo) && ((SelectedContact as UserUiInfo).UserId == sender.Id) )
                {
                    SelectedContactChanged();
                }
            }
            else if (message is GroupMessage)
            {
                if (ContactsList.Where(c => c is ChatGroupUiInfo).FirstOrDefault(x => (x as ChatGroupUiInfo).ChatGroupId == (message as GroupMessage).ChatGroupId) == null)
                {
                    UserServiceClient.AddUserToChatGroupContactAsync((message as GroupMessage).ChatGroupId, GlobalBase.CurrentUser.Id).ContinueWith(task =>
                    {
                        UpdateContactList();
                    });
                }
                else if ( (SelectedContact is ChatGroupUiInfo) && ((SelectedContact as ChatGroupUiInfo).ChatGroupId == (message as GroupMessage).ChatGroupId) )
                {
                    SelectedContactChanged();
                }
            }
        }

        public override void UserLeave(User user)
        {
            UpdateContactList();
            Debug.WriteLine("Works(Leave) - " + user.FirstName + " - (currentUser - " + GlobalBase.CurrentUser.FirstName + ")");
        }

        public override void UserCame(User user)
        {
            UpdateContactList();
            Debug.WriteLine("Works(Came) - " + user.FirstName + " - (currentUser - " + GlobalBase.CurrentUser.FirstName + ")");
        }

        public override void OnMessageRemoved(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public override void OnMessageEdited(BaseMessage message)
        {
            //throw new NotImplementedException();
        }
    }
}