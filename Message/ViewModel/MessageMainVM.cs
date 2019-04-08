using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Message.Compression;
using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Microsoft.Win32;
using Prism.Commands;
using User = Message.UserServiceReference.User;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class MessageMainVM : BaseViewModel
    {
        private int messageLimit = 50;

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

        private string _searchContactStr;

        public string SearchContactStr
        {
            get { return _searchContactStr; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    ContactsList.Clear();
                    List<UiInfo> temp = new List<UiInfo>();
                    temp.AddRange(UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id));

                    ContactsList = temp;

                    GlobalBase.loadPictures(UserServiceClient, ContactsList);
                }
                else
                {
                    ContactsList.Clear();
                    List<UiInfo> temp = new List<UiInfo>();
                    temp.AddRange(UserServiceClient
                        .GetAllUsersContacts(GlobalBase.CurrentUser.Id)
                        .Where(i => i.FirstName.Contains(value)
                                    || i.LastName.Contains(value)
                                    || i.Login.Contains(value))
                        .Select(u => new UserUiInfo
                        {
                            Name = u.FirstName + " " + u.LastName,
                            UniqueName = u.Login,
                            UserId = u.Id,
                            Status = u.Status
                        }));

                    ContactsList = temp;

                    GlobalBase.loadPictures(UserServiceClient, ContactsList);
                }

                SetProperty(ref _searchContactStr, value);
            }
        }

        private string _currentUserName;

        public string CurrentUserName
        {
            get { return GlobalBase.CurrentUser.FirstName + " " + GlobalBase.CurrentUser.LastName; }
            set { SetProperty(ref _currentUserName, value); }
        }

        private string _currentUserLogin;

        public string CurrentUserLogin
        {
            get { return "@" + GlobalBase.CurrentUser.Login; }
            set { SetProperty(ref _currentUserLogin, value); }
        }

        private bool _isDialogSearchVisible;

        public bool IsDialogSearchVisible
        {
            get { return _isDialogSearchVisible; }
            set { SetProperty(ref _isDialogSearchVisible, value); }
        }

        private List<UiInfo> _contactsList = new List<UiInfo>();

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
                if (value != null)
                {
                    SetProperty(ref _selectedContact, value, () => { SelectedContactChanged(); });
                }
            }
        }

        private string _dialogSearchStr;

        public string DialogSearchStr
        {
            get { return _dialogSearchStr; }
            set
            {
                SetProperty(ref _dialogSearchStr, value);
                ExecuteOnDialogSearch();
            }
        }

        private string _messageText;

        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); }
        }

        private string[] _filesPath;

        public string[] FilesPath
        {
            get { return _filesPath; }
            set { SetProperty(ref _filesPath, value); }
        }

        private bool _isMenuEnabled = false;

        public bool IsMenuEnabled
        {
            get { return _isMenuEnabled; }
            set { SetProperty(ref _isMenuEnabled, value); }
        }

        private int? _fileAmount;

        public int? FileAmount
        {
            get
            {
                if (_fileAmount == 0)
                {
                    return null;
                }

                return _fileAmount;
            }
            set { SetProperty(ref _fileAmount, value); }
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

        private DelegateCommand _createChatGroup;

        public DelegateCommand CreateChatGroup =>
            _createChatGroup ?? (_createChatGroup = new DelegateCommand(ExecuteOnCreateChatGroup));

        public MessageMainVM(IMessaging View, User user) : base()
        {
            _view = View;
            _view.ScrolledToTop += OnScrolledToTop;

            GlobalBase.CurrentUser = user;
            GlobalBase.UpdateMessagesOnUI += () =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    _view.UpdateMessageList();
                }));
            };
            GlobalBase.AddMessageOnUi += AddMessageOnUI;

            GlobalBase.UpdateContactList += UpdateContactList;
            GlobalBase.RemoveMessageOnUI += DeleteMessageOnUI;
            GlobalBase.UpdateProfileUi += SetAvatarForUI;

            SetAvatarForUI();
            UpdateContactList();
            UserServiceClient.OnUserCame(user.Id);

            IsMenuEnabled = false;
            FileAmount = 0;
        }

        //update data using server
        private void UpdateContactList()
        {
            Task.Run(() =>
            {
                var temp = SelectedContact;

                var tempUiInfos = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);
                GlobalBase.loadPictures(UserServiceClient, tempUiInfos);

                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    lock (GlobalBase.contactsMonitor)
                    {
                        ContactsList.Clear();
                        ContactsList = tempUiInfos;
                    }
                });

                if (temp != null)
                {
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        lock (GlobalBase.contactsMonitor)
                        {
                            SelectedContact = ContactsList.FirstOrDefault(ct => ct.UniqueName == temp.UniqueName);
                        }
                    });
                }
            });
        }

        private void SelectedContactChanged(object sender = null, PropertyChangedEventArgs e = null)
        {
            if (SelectedContact != null)
            {
                IsMenuEnabled = true;
            }
            else
            {
                IsMenuEnabled = false;
            }

            if (_view.MessageList != null)
            {
                Task.Run(() =>
                {
                    _view.MessageList.Clear();

                    GlobalBase.loadPictures(UserServiceClient, ContactsList);

                    var res = new List<BaseMessage>();
                    if (SelectedContact is UserUiInfo)
                    {
                        res.AddRange(UserServiceClient.GetUserMessages(GlobalBase.CurrentUser.Id,
                            (SelectedContact as UserUiInfo).UserId, messageLimit));
                    }
                    else if (SelectedContact is ChatGroupUiInfo)
                    {
                        res.AddRange(
                            UserServiceClient.GetGroupMessages((SelectedContact as ChatGroupUiInfo).ChatGroupId, messageLimit));
                    }

                    if (res.Count != 0)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            _view.MessageList.AddRange(res);
                        }));
                    }
                }).ContinueWith((task =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        GlobalBase.UpdateMessagesOnUI();
                    }));
                }));
            }
        }

        private void UpdateMessages(BaseMessage message, Func<BaseMessage, bool> updateStrategy)
        {
            var sender = UserServiceClient.GetAllUsers().FirstOrDefault(x => x.Id == message.SenderId);

            if (message is GroupMessage gMes)
            {
                gMes.SenderName = sender.FirstName;
            }

            if (sender.Id != (SelectedContact as UserUiInfo)?.UserId)
            {
                var mes = "New message from  @" + sender.Login + "\n" + "\"" + GlobalBase.Base64Decode(message.Text) +
                          "\"";
                GlobalBase.ShowNotify("New message", mes);
            }

            Debug.WriteLine("Receave Message from - ", sender.Login);

            if (message is UserMessage)
            {
                if ((SelectedContact is UserUiInfo) && ((SelectedContact as UserUiInfo).UserId == sender.Id))
                {
                    updateStrategy(message);
                }
            }
            else if (message is GroupMessage)
            {
                if ((SelectedContact is ChatGroupUiInfo) && ((SelectedContact as ChatGroupUiInfo).ChatGroupId == (message as GroupMessage).ChatGroupId))
                {
                    updateStrategy(message);
                }
            }
        }

        private void OnScrolledToTop()
        {
            //TODO
        }

        //UI update
        private bool AddMessageOnUI(BaseMessage message)
        {
            lock (_view.MessageList)
            {
                Dispatcher.CurrentDispatcher.Invoke(() => { GlobalBase.AddMessageOnUi.Invoke(message); });
            }

            GlobalBase.UpdateMessagesOnUI.Invoke();
            return true;
        }

        private bool UppdateMessageOnUI(BaseMessage message)
        {
            var mes = _view.MessageList.FirstOrDefault(x => x.Id == message.Id);
            if (mes != null)
            {
                mes.Text = message.Text;
                GlobalBase.UpdateMessagesOnUI.Invoke();
                return true;
            }

            return false;
        }

        private bool DeleteMessageOnUI(BaseMessage message)
        {
            var mes = _view.MessageList.FirstOrDefault(x => x.Id == message.Id);
            if (mes != null)
            {
                _view.MessageList.Remove(mes);
                GlobalBase.UpdateMessagesOnUI.Invoke();
                return true;
            }

            return false;
        }

        private void SetAvatarForUI()
        {
            Task.Run(() =>
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    Images = GlobalBase.getUsersAvatar(GlobalBase.CurrentUser);
                });
            });
        }

        //executes for commands
        private void ExecuteOnAddFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.ShowDialog();
            FilesPath = openFileDialog.FileNames;

            FileAmount = FilesPath.Count();
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
                var userUiInfo = SelectedContact as UserUiInfo;
                var user = UserServiceClient.GetUserById(userUiInfo.UserId);
                var wnd = new ContactProfileWindow(user);
                wnd.Owner = (Window)_view;
                wnd.Show();
            }
            else if (SelectedContact is ChatGroupUiInfo chatGroupUiInfo)
            {
                var wnd = new EditGroupWindow(chatGroupUiInfo);
                wnd.Owner = (Window)_view;
                wnd.Show();
            }
        }

        private void ExecuteOnSendMessage()
        {
            if (SelectedContact != null && (!string.IsNullOrWhiteSpace(MessageText) || FilesPath != null))
            {
                BaseMessage message = null;
                List<BaseMessage> messagesWithFile = null;
                if (SelectedContact is UserUiInfo)
                {
                    var userUiInfo = SelectedContact as UserUiInfo;
                    if (FilesPath == null)
                    {
                        message = new UserMessage()
                        {
                            Text = Encoding.UTF8.GetBytes(MessageText),
                            DateOfSending = DateTime.Now,
                            SenderId = GlobalBase.CurrentUser.Id,
                            Sender = GlobalBase.CurrentUser,
                            ReceiverId = userUiInfo.UserId,
                        };
                    }
                    else
                    {
                        if (FilesPath.Length == 1)
                        {
                            var chatFile = new Message.FileService.ChatFile() { Source = CompressionHelper.CompressFile(GlobalBase.FileToByte(FilesPath[0])), Name = GlobalBase.GetShortName(FilesPath[0]) };

                            var tempMes = new UserMessage()
                            {
                                Text = MessageText != null ? Encoding.UTF8.GetBytes(MessageText) : Encoding.UTF8.GetBytes(chatFile.Name),
                                ChatFileId = GlobalBase.FileServiceClient.UploadFile(chatFile),
                                DateOfSending = DateTime.Now,
                                SenderId = GlobalBase.CurrentUser.Id,
                                Sender = GlobalBase.CurrentUser,
                                ReceiverId = userUiInfo.UserId,
                            };
                            UserServiceClient.SendMessageAsync(tempMes).ContinueWith(task => _view.MessageList.Add(UserServiceClient.GetLastMessage()));
                        }
                        else
                        {
                            if (MessageText != null)
                            {
                                var tempMes = new UserMessage()
                                {
                                    Text = Encoding.UTF8.GetBytes(MessageText),
                                    DateOfSending = DateTime.Now,
                                    SenderId = GlobalBase.CurrentUser.Id,
                                    Sender = GlobalBase.CurrentUser,
                                    ReceiverId = userUiInfo.UserId,
                                };
                                UserServiceClient.SendMessageAsync(tempMes).ContinueWith(task => _view.MessageList.Add(UserServiceClient.GetLastMessage()));
                            }

                            messagesWithFile = new List<BaseMessage>();
                            foreach (var file in FilesPath)
                            {
                                var chatFile = new Message.FileService.ChatFile() { Source = CompressionHelper.CompressFile(GlobalBase.FileToByte(file)), Name = GlobalBase.GetShortName(file) };

                                messagesWithFile.Add(new UserMessage()
                                {
                                    Text = Encoding.UTF8.GetBytes(chatFile.Name),
                                    ChatFileId = GlobalBase.FileServiceClient.UploadFile(chatFile),
                                    DateOfSending = DateTime.Now,
                                    SenderId = GlobalBase.CurrentUser.Id,
                                    Sender = GlobalBase.CurrentUser,
                                    ReceiverId = userUiInfo.UserId,
                                });
                            }
                        }
                    }
                }
                else if (SelectedContact is ChatGroupUiInfo)
                {
                    var userUiInfo = SelectedContact as ChatGroupUiInfo;
                    if (FilesPath == null)
                    {
                        message = new GroupMessage()
                        {
                            Text = Encoding.UTF8.GetBytes(MessageText),
                            DateOfSending = DateTime.Now,
                            SenderId = GlobalBase.CurrentUser.Id,
                            Sender = GlobalBase.CurrentUser,
                            ChatGroupId = userUiInfo.ChatGroupId,
                        };
                    }
                    else
                    {
                        var tempMes = new GroupMessage()
                        {
                            Text = Encoding.UTF8.GetBytes(MessageText),
                            DateOfSending = DateTime.Now,
                            SenderId = GlobalBase.CurrentUser.Id,
                            Sender = GlobalBase.CurrentUser,
                            ChatGroupId = userUiInfo.ChatGroupId,
                        };
                        UserServiceClient.SendMessageAsync(tempMes).ContinueWith(task => _view.MessageList.Add(UserServiceClient.GetLastMessage()));

                        messagesWithFile = new List<BaseMessage>();
                        foreach (var file in FilesPath)
                        {
                            var chatFile = new Message.FileService.ChatFile() { Source = CompressionHelper.CompressFile(GlobalBase.FileToByte(file)), Name = GlobalBase.GetShortName(file) };

                            messagesWithFile.Add(new GroupMessage()
                            {
                                Text = Encoding.UTF8.GetBytes(chatFile.Name),
                                ChatFileId = GlobalBase.FileServiceClient.UploadFile(chatFile),
                                DateOfSending = DateTime.Now,
                                SenderId = GlobalBase.CurrentUser.Id,
                                Sender = GlobalBase.CurrentUser,
                                ChatGroupId = userUiInfo.ChatGroupId,
                            });
                        }
                    }
                }

                if (FilesPath == null)
                {
                    UserServiceClient.SendMessageAsync(message).ContinueWith(task =>
                    {
                        var lastMessage = UserServiceClient.GetLastMessage();
                        _view.MessageList.Add(lastMessage);
                        GlobalBase.UpdateMessagesOnUI();
                    });
                }
                else if (messagesWithFile != null)
                {
                    foreach (var fileMessage in messagesWithFile)
                    {
                        UserServiceClient.SendMessage(fileMessage);
                        _view.MessageList.Add(UserServiceClient.GetLastMessage());
                    }
                }

                FilesPath = null;
                FileAmount = 0;

                MessageText = string.Empty;

                Debug.WriteLine("Send Message");
            }
        }

        private void ExecuteOnSettingsCommand()
        {
            _view.SetOpacity(0.5);

            var wnd = new SettingsWindow();
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();

            UpdateContactList();

            _view.SetOpacity(1);
        }

        private void ExecuteOnContacts()
        {
            _view.SetOpacity(0.5);

            var wnd = new Contacts();
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();

            _view.SetOpacity(1);

            UpdateContactList();
        }

        private void ExecuteOnCreateChatGroup()
        {
            _view.SetOpacity(0.5);

            var wnd = new CreateChatGroupWnd();
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();

            _view.SetOpacity(1);

            UpdateContactList();
        }

        private void ExecuteOnDialogSearch()
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
                                    Id = mes.Id,
                                    Text = mes.Text,
                                    DateOfSending = mes.DateOfSending,
                                    ReceiverId = userMessage.ReceiverId,
                                    SenderId = mes.SenderId,
                                };
                            }
                            else if (mes is GroupMessage)
                            {
                                GroupMessage groupMessage = mes as GroupMessage;
                                message = new GroupMessage()
                                {
                                    Id = mes.Id,
                                    Text = mes.Text,
                                    DateOfSending = mes.DateOfSending,
                                    ChatGroupId = groupMessage.ChatGroupId,
                                    SenderId = mes.SenderId,
                                };
                            }

                            if (GlobalBase.Base64Decode(mes.Text).Contains(DialogSearchStr))
                            {
                                GlobalBase.AddMessageOnUi.Invoke(message);
                            }
                        }
                        GlobalBase.UpdateMessagesOnUI();
                    }
                }
            }
            else
            {
                SelectedContactChanged();
            }
        }

        //service callbacks
        public override void ReceiveMessage(BaseMessage message)
        {
            UpdateMessages(message, AddMessageOnUI);
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
            DeleteMessageOnUI(message);
        }

        public override void OnMessageEdited(BaseMessage message)
        {
            UpdateMessages(message, UppdateMessageOnUI);
        }

        public override void OnNewContactAdded(UiInfo newContact)
        {
            UiInfo foundedUiInfo = ContactsList.FirstOrDefault(c => c.UniqueName == newContact.UniqueName);

            if (foundedUiInfo == null)
            {
                List<UiInfo> temp = new List<UiInfo>();
                temp.AddRange(ContactsList);

                GlobalBase.loadPicture(UserServiceClient, newContact);
                temp.Add(newContact);

                ContactsList = temp;
            }
        }

        public override void OnContactRemoved(UiInfo newContact)
        {
            UiInfo foundedUiInfo = ContactsList.FirstOrDefault(c => c.UniqueName == newContact.UniqueName);

            if (foundedUiInfo != null)
            {
                List<UiInfo> temp = new List<UiInfo>();
                temp.AddRange(ContactsList);

                ContactsList.Remove(foundedUiInfo);
                ContactsList = temp;
            }
        }

        public override void OnEntityChanged(UiInfo changedEntity)
        {
            if (changedEntity.UniqueName == GlobalBase.CurrentUser.Login)
            {
                GlobalBase.UpdateProfileUi.Invoke();
            }
            else
            {
                foreach(UiInfo uiInfo in ContactsList)
                {
                    if (uiInfo.UniqueName == changedEntity.UniqueName)
                    {
                        GlobalBase.UpdateContactList();
                        break;
                    }
                }
            }
        }

    }
}