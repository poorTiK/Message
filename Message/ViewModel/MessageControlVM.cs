using Message.AdditionalItems;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.ServiceModel;
using System.Windows;

namespace Message.ViewModel
{
    internal class MessageControlVM : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        private InstanceContext usersSite;
        private UserServiceClient userServiceClient;
        private IUserServiceCallback _userServiceCallback;

        private BaseMessage Message { get; set; }

        private string MessageText
        {
            get
            {
                return GlobalBase.Base64Decode(Message.Content);
            }
            set { }
        }

        public MessageControlVM(BaseMessage message)
        {
            _userServiceCallback = this;
            usersSite = new InstanceContext(_userServiceCallback);
            userServiceClient = new UserServiceClient(usersSite);

            Message = message;
        }

        private DelegateCommand _onCopy;

        public DelegateCommand Copy =>
            _onCopy ?? (_onCopy = new DelegateCommand(OnCopy));

        private DelegateCommand _onDelete;

        public DelegateCommand Delete =>
            _onDelete ?? (_onDelete = new DelegateCommand(OnDelete));

        private DelegateCommand _onEdit;

        public DelegateCommand Edit =>
            _onEdit ?? (_onEdit = new DelegateCommand(OnEdit));

        private DelegateCommand _onForward;

        public DelegateCommand Forward =>
            _onForward ?? (_onForward = new DelegateCommand(OnForward));

        private void OnForward()
        {
            var wnd = new ForwardMessageWindow(Message);
            wnd.ShowDialog();

            GlobalBase.UpdateUI.Invoke();
        }

        private void OnEdit()
        {
            var wnd = new MessageEditWindow(Message);
            wnd.ShowDialog();

            GlobalBase.UpdateUI.Invoke();
        }

        private void OnDelete()
        {
            var canDelete = CustomMessageBox.Show("", "Are you sure want to\n delete message?", MessageBoxButton.YesNo);

            switch (canDelete)
            {
                case MessageBoxResult.Yes:
                    userServiceClient.RemoveMessage(Message);

                    GlobalBase.UpdateUI.Invoke();
                    break;

                case MessageBoxResult.No:
                    break;
            }
        }

        private void OnCopy()
        {
            Clipboard.SetText(MessageText);
        }

        public void UserLeave(User user)
        {
            //throw new NotImplementedException();
        }

        public void UserCame(User user)
        {
            //throw new NotImplementedException();
        }

        public void ReceiveMessage(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public void OnMessageRemoved(BaseMessage message)
        {
            //throw new System.NotImplementedException();
        }

        public void OnMessageEdited(BaseMessage message)
        {
            //throw new System.NotImplementedException();
        }
    }
}