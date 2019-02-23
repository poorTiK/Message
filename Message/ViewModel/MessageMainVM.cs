using Message.Interfaces;
using Message.MessageServiceReference;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Windows;
using MessageT = Message.MessageServiceReference.MessageT;

namespace Message.ViewModel
{
    internal class MessageMainVM : Prism.Mvvm.BindableBase, IMessageServiceCallback
    {
        private UserServiceClient userServiceClient;

        private InstanceContext site;
        private MessageServiceClient _messageServiceClient;
        private IMessageServiceCallback _messageServiceCallback;
        private const string HOST = "192.168.0.255";
        private IPAddress groupAddress;

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

        public MessageMainVM(IMessaging View)
        {
            _view = View;

            userServiceClient = new UserServiceClient();
        }

        public MessageMainVM(IMessaging View, User user)
        {
            _view = View;
            CurrentUser = user;
            GlobalBase.CurrentUser = user;

            userServiceClient = new UserServiceClient();

            //callback
            groupAddress = IPAddress.Parse(HOST);
            _messageServiceCallback = this;
            site = new InstanceContext(_messageServiceCallback);
            _messageServiceClient = new MessageServiceClient(site);

            ContactsList = userServiceClient.GetAllContacts(GlobalBase.CurrentUser);
            SelectedContact = new User();
        }

        private void SelectedContactChanged()
        {
            if (_view.MessageList != null)
            {
                _view.MessageList.Clear();
                var res = userServiceClient.GetMessages(GlobalBase.CurrentUser, SelectedContact, 50);
                if (res != null)
                {
                    foreach (var mes in res)
                    {
                        _view.MessageList.Add(new MessageT()
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

        private void ExecuteOnSendMessage()
        {
            if (SelectedContact != null && !string.IsNullOrWhiteSpace(MessageText))
            {
                var message = new MessageT()
                {
                    Content = Encoding.UTF8.GetBytes(MessageText),
                    DateOfSending = DateTime.Now,
                    SenderId = GlobalBase.CurrentUser.Id,
                    ReceiverId = SelectedContact.Id,
                    Type = "TEXT",
                };

                _messageServiceClient.SendMessage(message);
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
                            var message = new MessageT()
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
            ContactsList = userServiceClient.GetAllContacts(GlobalBase.CurrentUser);
        }

        public void ReceiveMessage(MessageT message)
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
    }
}