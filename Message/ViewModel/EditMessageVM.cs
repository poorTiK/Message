﻿using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System.Text;

namespace Message.ViewModel
{
    internal class EditMessageVM : BaseViewModel
    {
        private IView _view;

        private BaseMessage Message { get; set; }

        private string _messageText;

        public string MessageText
        {
            get { return _messageText; }
            set { SetProperty(ref _messageText, value, () => Validate()); }
        }

        private bool isApplyEnabled;

        public bool IsApplyEnabled
        {
            get { return isApplyEnabled; }
            set { SetProperty(ref isApplyEnabled, value); }
        }

        public EditMessageVM(BaseMessage message, IView View) : base()
        {
            _view = View;

            Message = message;
            MessageText = GlobalBase.Base64Decode(message.Text);
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
            Message.Text = Encoding.UTF8.GetBytes(MessageText);

            UserServiceClient.EditMessage(Message);

            _view.CloseWindow();
        }

        private void Validate()
        {
            IsApplyEnabled = !string.IsNullOrWhiteSpace(MessageText);
        }
    }
}