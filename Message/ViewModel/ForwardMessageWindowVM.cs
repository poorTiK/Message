using Message.UserServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Message.Interfaces;
using System.ServiceModel;
using Message.Model;
using Prism.Commands;

namespace Message.ViewModel
{
    class ForwardMessageWindowVM : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        private InstanceContext usersSite;
        private UserServiceClient userServiceClient;
        private IUserServiceCallback _userServiceCallback;

        private IView _view;
        private BaseMessage _message;

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
            set { SetProperty(ref _selectedContact, value); }
        }

        private string _messageText;
        public string MessageText
        {
            get { return GlobalBase.Base64Decode(_message.Content); }
        }

        public ForwardMessageWindowVM(BaseMessage message, IView view)
        {
            _userServiceCallback = this;
            usersSite = new InstanceContext(_userServiceCallback);
            userServiceClient = new UserServiceClient(usersSite);

            ContactsList = userServiceClient.GetAllContacts(GlobalBase.CurrentUser);

            _view = view;

            _message = message;
        }

        private DelegateCommand _onForward;

        public DelegateCommand Forward =>
            _onForward ?? (_onForward = new DelegateCommand(OnForward));

        private DelegateCommand _onBack;

        public DelegateCommand Back =>
            _onBack ?? (_onBack = new DelegateCommand(OnBack));

        private void OnBack()
        {
            _view.CloseWindow();
        }

        private void OnForward()
        {
            if (SelectedContact != null)
            {
                var mes = new UserMessage()
                {
                    Content = _message.Content,
                    DateOfSending = _message.DateOfSending,
                    SenderId = _message.SenderId,
                    Type = _message.Type,
                    ReceiverId = SelectedContact.Id
                };
                userServiceClient.SendMessageAsync(mes);
            }
            _view.CloseWindow();
        }

        public void OnMessageEdited(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public void OnMessageRemoved(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public void ReceiveMessage(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public void UserCame(User user)
        {
            //throw new NotImplementedException();
        }

        public void UserLeave(User user)
        {
            //throw new NotImplementedException();
        }
    }
}
