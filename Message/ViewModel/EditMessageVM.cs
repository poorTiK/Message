using Message.UserServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Message.Interfaces;
using Message.Model;
using Prism.Commands;

namespace Message.ViewModel
{
    class EditMessageVM : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        private InstanceContext usersSite;
        private UserServiceClient userServiceClient;
        private IUserServiceCallback _userServiceCallback;

        private IView _view;

        private BaseMessage Message { get; set; }

        private string _messageText;
        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value); }
        }

        public EditMessageVM(BaseMessage message, IView View)
        {
            _userServiceCallback = this;
            usersSite = new InstanceContext(_userServiceCallback);
            userServiceClient = new UserServiceClient(usersSite);

            _view = View;

            Message = message;
            MessageText = GlobalBase.Base64Decode(message.Content);
        }

        private DelegateCommand _onApply;

        public DelegateCommand Apply =>
            _onApply ?? (_onApply = new DelegateCommand(OnApply));

        private DelegateCommand _onBack;

        public DelegateCommand Back =>
            _onBack ?? (_onBack = new DelegateCommand(OnBack));

        private void OnBack()
        {
            _view.CloseWindow();
        }

        private void OnApply()
        {
            Message.Content = Encoding.UTF8.GetBytes(MessageText);

            userServiceClient.EditMessage(Message);

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
