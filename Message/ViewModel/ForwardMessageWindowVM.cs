using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System.Collections.Generic;

namespace Message.ViewModel
{
    internal class ForwardMessageWindowVM : BaseViewModel
    {
        private IView _view;
        private BaseMessage _message;

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
            set { SetProperty(ref _selectedContact, value); }
        }

        private string _messageText;

        public string MessageText
        {
            get { return GlobalBase.Base64Decode(_message.Text); }
        }

        public ForwardMessageWindowVM(BaseMessage message, IView view) : base()
        {
            List<UiInfo> uiInfos = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);
            GlobalBase.loadPictures(UserServiceClient, uiInfos);
            ContactsList = uiInfos;

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
            if (SelectedContact != null && SelectedContact is UserUiInfo)
            {
                UserUiInfo userUiInfo = SelectedContact as UserUiInfo;
                var mes = new UserMessage()
                {
                    Text = _message.Text,
                    DateOfSending = _message.DateOfSending,
                    SenderId = _message.SenderId,
                    ReceiverId = userUiInfo.UserId
                };
                UserServiceClient.SendMessageAsync(mes);
            }
            _view.CloseWindow();
        }
    }
}