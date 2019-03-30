using Message.Compression;
using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Microsoft.Win32;
using Prism.Commands;
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
                SetProperty(ref _selectedContact, value, () => { SelectedContactChanged(); });
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

        private string[] _filesPath;

        public string[] FilesPath
        {
            get { return _filesPath; }
            set { SetProperty(ref _filesPath, value); }
        }

        private bool _isMenuEnabled;

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
            Update();

            IsMenuEnabled = false;
            FileAmount = 0;

            UserServiceClient.OnUserCame(user.Id);
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
                _view.MessageList.Clear();

                Task.Run(() =>
                {
                    GlobalBase.loadPictures(UserServiceClient, ContactsList);

                    List<BaseMessage> res = new List<BaseMessage>();
                    if (SelectedContact is UserUiInfo)
                    {
                        res.AddRange(UserServiceClient.GetUserMessages(GlobalBase.CurrentUser.Id,
                            (SelectedContact as UserUiInfo).UserId, 50));
                    }
                    else if (SelectedContact is ChatGroupUiInfo)
                    {
                        res.AddRange(
                            UserServiceClient.GetGroupMessages((SelectedContact as ChatGroupUiInfo).ChatGroupId, 50));
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
                    Application.Current.Dispatcher.Invoke(new Action(() => { _view.UpdateMessageList(); }));
                }));
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

        private DelegateCommand _createChatGroup;

        public DelegateCommand CreateChatGroup =>
            _createChatGroup ?? (_createChatGroup = new DelegateCommand(ExecuteOnCreateChatGroup));

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
                wnd.ShowDialog();
            }
            else if (SelectedContact is ChatGroupUiInfo)
            {
                //TODO
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
                    UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
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
                            UserServiceClient.SendMessageAsync(tempMes);

                            _view.MessageList.Add(tempMes);
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
                                UserServiceClient.SendMessageAsync(tempMes);

                                _view.MessageList.Add(tempMes);
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
                    ChatGroupUiInfo userUiInfo = SelectedContact as ChatGroupUiInfo;
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
                        UserServiceClient.SendMessageAsync(tempMes);

                        _view.MessageList.Add(tempMes);

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
                    UserServiceClient.SendMessageAsync(message);
                    _view.MessageList.Add(message);
                }
                else if (messagesWithFile != null)
                {
                    foreach (var fileMessage in messagesWithFile)
                    {
                        UserServiceClient.SendMessage(fileMessage);
                        //var mes = UserServiceClient.GetUserMessages(GlobalBase.CurrentUser.Id,
                        //    (SelectedContact as UserUiInfo).UserId, 1);
                        //GlobalBase.PhotoServiceClient.SetFileToMessage(mes.Last().Id, fileMessage.Text);
                        //fileMessage.Id = mes.Last().Id;
                        //fileMessage.Text = null;
                        _view.MessageList.Add(fileMessage);
                    }
                }

                FilesPath = null;
                FileAmount = 0;

                _view.UpdateMessageList();

                MessageText = string.Empty;

                Debug.WriteLine("Send Message");
            }
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

        private void ExecuteOnCreateChatGroup()
        {
            _view.SetOpacity(0.5);

            var wnd = new CreateChatGroupWnd();
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

        private void UpdateContactList()
        {
            Task.Run(() =>
            {
                UiInfo temp = SelectedContact;

                List<UiInfo> tempUiInfos = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);
                GlobalBase.loadPictures(UserServiceClient, tempUiInfos);
                ContactsList = tempUiInfos;

                if (temp is UserUiInfo)
                {
                    UserUiInfo userUiInfo = temp as UserUiInfo;

                    if (ContactsList.Any(x => temp != null && (x is UserUiInfo) && ((x as UserUiInfo).UserId == userUiInfo.UserId)))
                    {
                        Dispatcher.CurrentDispatcher.Invoke(() => { SelectedContact = temp; });
                    }
                }
                else if (temp is ChatGroupUiInfo)
                {
                    ChatGroupUiInfo chatGroupUiInfo = temp as ChatGroupUiInfo;

                    if (ContactsList.Any(x =>
                        temp != null && ((x is ChatGroupUiInfo) && (x as ChatGroupUiInfo).ChatGroupId == chatGroupUiInfo.ChatGroupId)))
                    {
                        Dispatcher.CurrentDispatcher.Invoke(() => { SelectedContact = temp; });
                    }
                }
            });
        }

        public override void ReceiveMessage(BaseMessage message)
        {
            UpdateMessages(message, AddMessage);
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
            UpdateMessages(message, DeleteMessage);
        }

        public override void OnMessageEdited(BaseMessage message)
        {
            UpdateMessages(message, UppdateMessage);
        }

        private bool AddMessage(BaseMessage message)
        {
            _view.MessageList.Add(message);
            _view.UpdateMessageList();

            return true;
        }

        private bool UppdateMessage(BaseMessage message)
        {
            var mes = _view.MessageList.FirstOrDefault(x => x.Id == message.Id);
            if (mes != null)
            {
                mes.Text = message.Text;
                _view.UpdateMessageList();
                return true;
            }

            return false;
        }

        private bool DeleteMessage(BaseMessage message)
        {
            var mes = _view.MessageList.FirstOrDefault(x => x.Id == message.Id);
            if (mes != null)
            {
                _view.MessageList.Remove(mes);
                _view.UpdateMessageList();
                return true;
            }

            return false;
        }

        private void UpdateMessages(BaseMessage message, Func<BaseMessage, bool> updateMethod)
        {
            User sender = UserServiceClient.GetAllUsers().FirstOrDefault(x => x.Id == message.SenderId);

            if (sender.Id != (SelectedContact as UserUiInfo)?.UserId)
            {
                var mes = "New message from  @" + sender.Login + "\n" + "\"" + GlobalBase.Base64Decode(message.Text) +
                          "\"";
                GlobalBase.ShowNotify("New message", mes);
            }

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
                else if ((SelectedContact is UserUiInfo) && ((SelectedContact as UserUiInfo).UserId == sender.Id))
                {
                    updateMethod(message);
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
                else if ((SelectedContact is ChatGroupUiInfo) && ((SelectedContact as ChatGroupUiInfo).ChatGroupId == (message as GroupMessage).ChatGroupId))
                {
                    updateMethod(message);
                }
            }
        }
    }
}