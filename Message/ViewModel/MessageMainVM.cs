using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Message.MessageServiceReference;
using MessageT = Message.MessageServiceReference.MessageT;
using System.ServiceModel;
using System.Text;

namespace Message.ViewModel
{
    class MessageMainVM : Prism.Mvvm.BindableBase, IMessageServiceCallback
    {
        private UserServiceClient userServiceClient;
        private MessageServiceClient _messageServiceClient;
        private IMessageServiceCallback _messageServiceCallback;
        private InstanceContext site;

        IMessaging _view;

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

        private DelegateCommand _onContactsCommand;
        public DelegateCommand ContactsCommand =>
            _onContactsCommand ?? (_onContactsCommand = new DelegateCommand(ExecuteOnContacts));

        private DelegateCommand _onSettingsCommand;
        public DelegateCommand SettingsCommand =>
            _onSettingsCommand ?? (_onSettingsCommand = new DelegateCommand(ExecuteOnSettingsCommand));

        private DelegateCommand _dialogSearchCommand;
        public DelegateCommand DialogSearchCommand =>
            _dialogSearchCommand ?? (_dialogSearchCommand = new DelegateCommand(() => { IsDialogSearchVisible = !IsDialogSearchVisible; }));

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

        public void Update()
        {
            ContactsList = userServiceClient.GetAllContacts(GlobalBase.CurrentUser);
        }

        public void ReceiveMessage(MessageT message)
        {
            
        }
    }
}
