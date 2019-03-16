using Message.AdditionalItems;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.ServiceModel;
using System.Windows;

namespace Message.ViewModel
{
    internal class MessageControlVM : BaseViewModel
    {
        private BaseMessage Message { get; set; }

        private string MessageText
        {
            get
            {
                return GlobalBase.Base64Decode(Message.Content);
            }
            set { }
        }

        public MessageControlVM(BaseMessage message) : base()
        {
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
            var canDelete = CustomMessageBox.Show("", Application.Current.Resources.MergedDictionaries[4]["AskForDelMessage"].ToString(), MessageBoxButton.YesNo);

            switch (canDelete)
            {
                case MessageBoxResult.Yes:
                    UserServiceClient.RemoveMessage(Message);

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
    }
}