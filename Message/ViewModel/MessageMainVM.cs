using Message.Interfaces;
using Message.UserServiceReference;
using Message.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Diagnostics;
using Microsoft.Win32;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class MessageMainVM : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        private const string HOST = "192.168.0.255";
        private IPAddress groupAddress;

        private InstanceContext usersSite;
        private UserServiceClient userServiceClient;
        private IUserServiceCallback _userServiceCallback;

        private IMessaging _view;

        public User CurrentUser { get; set; }

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

        private List<User> _contactsList;

        public List<User> ContactsList
        {
            get { return _contactsList; }
            set { SetProperty(ref _contactsList, value); }
        }

        private User _selectedContact;

        public User SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                SetProperty(ref _selectedContact, value);
                SelectedContactChanged();
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

        public MessageMainVM(IMessaging View)
        {
            _view = View;
            
            //callback for user
            _userServiceCallback = this;
            usersSite = new InstanceContext(_userServiceCallback);
            userServiceClient = new UserServiceClient(usersSite);
        }

        public MessageMainVM(IMessaging View, User user) : this(View)
        {
            CurrentUser = user;
            GlobalBase.CurrentUser = user;

            //callback for messages
            groupAddress = IPAddress.Parse(HOST);

            ContactsList = userServiceClient.GetAllContacts(GlobalBase.CurrentUser);
            SelectedContact = new User();

            IsMenuEnabled = false;

            userServiceClient.OnUserCame(user);
        }

        private void SelectedContactChanged()
        {
            if (SelectedContact != null)
                IsMenuEnabled = true;
            else
                IsMenuEnabled = false;

            if (_view.MessageList != null)
            {
                _view.MessageList.Clear();
                var res = userServiceClient.GetMessages(GlobalBase.CurrentUser, SelectedContact, 50);
                if (res != null)
                {
                    foreach (var mes in res)
                    {
                        _view.MessageList.Add(new UserMessage()
                        {
                            Content = mes.Content,
                            DateOfSending = mes.DateOfSending,
                            ReceiverId = mes.ReceiverId,
                            SenderId = mes.SenderId,
                            Type = mes.Type
                        });
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
            userServiceClient.OnUserLeave(GlobalBase.CurrentUser);
            _view.CloseWindow();
        }

        private void ExecuteOnViewProfile()
        {
            var wnd = new ContactProfileWindow(SelectedContact);
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();

            Update();
        }

        private void ExecuteOnSendMessage()
        {
            if (SelectedContact != null && !string.IsNullOrWhiteSpace(MessageText))
            {
                var message = new UserMessage()
                {
                    Content = Encoding.UTF8.GetBytes(MessageText),
                    DateOfSending = DateTime.Now,
                    SenderId = GlobalBase.CurrentUser.Id,
                    ReceiverId = SelectedContact.Id,
                    Type = "TEXT",
                };

                userServiceClient.SendMessageAsync(message);
                _view.MessageList.Add(message);

                _view.UpdateMessageList();

                MessageText = string.Empty;
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
                    var res = userServiceClient.GetMessages(GlobalBase.CurrentUser, SelectedContact, 50);
                    if (res != null)
                    {
                        foreach (var mes in res)
                        {
                            var message = new UserMessage()
                            {
                                Content = mes.Content,
                                DateOfSending = mes.DateOfSending,
                                ReceiverId = mes.ReceiverId,
                                SenderId = mes.SenderId,
                                Type = mes.Type
                            };
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
            //todo check if contact still in contacts
            var temp = SelectedContact;

            ContactsList = userServiceClient.GetAllContacts(GlobalBase.CurrentUser);

            if (temp != null)
            {
                SelectedContact = temp;
            }
        }

        public void ReceiveMessage(UserMessage message)
        {
            if (message.SenderId == GlobalBase.CurrentUser.Id)
            {
                return;
            }
            else
            {
                var user = userServiceClient.GetAllUsers().FirstOrDefault(x => x.Id == message.SenderId);
                var mes = "New message from  @" + user.Login + "\n" + "\"" + GlobalBase.Base64Decode(message.Content) + "\"";
                GlobalBase.ShowNotify("New message", mes);
            }
        }

        public void UserLeave(User user)
        {
            Update();
            Debug.WriteLine("Works(Leave) - " + user.FirstName + " - (currentUser - " + GlobalBase.CurrentUser.FirstName + ")");
        }

        public void UserCame(User user)
        {
            Update();
            Debug.WriteLine("Works(Came) - " + user.FirstName + " - (currentUser - " + GlobalBase.CurrentUser.FirstName +  ")");
        }
    }
}