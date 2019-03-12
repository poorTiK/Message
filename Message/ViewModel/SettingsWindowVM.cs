using Message.Interfaces;
using Message.Model;
using Prism.Commands;
using System;
using System.Windows;

namespace Message.ViewModel
{
    internal class SettingsWindowVM : Prism.Mvvm.BindableBase
    {
        public IView view;

        private string _currentUserName;

        public string CurrentUserName
        {
            get { return GlobalBase.CurrentUser.FirstName + " " + GlobalBase.CurrentUser.LastName; }
            set { SetProperty(ref _currentUserName, value); }
        }

        private string _currentUserLogin;

        public string CurrentUserLogin
        {
            get { return "@" + GlobalBase.CurrentUser.Login; }
            set { SetProperty(ref _currentUserLogin, value); }
        }

        private byte[] _currentUserPhoto;

        public byte[] CurrentUserPhoto
        {
            get { return GlobalBase.CurrentUser.Avatar; }
            set { SetProperty(ref _currentUserPhoto, value); }
        }

        public SettingsWindowVM(IView view)
        {
            this.view = view;
        }

        private DelegateCommand _onProfileSettings;

        public DelegateCommand ProfileSettings =>
            _onProfileSettings ?? (_onProfileSettings = new DelegateCommand(ExecuteOnProfileSettings));

        private DelegateCommand _chatSettings;

        public DelegateCommand ChatSettings =>
            _chatSettings ?? (_chatSettings = new DelegateCommand(ExecuteOnChatSettings));

        private void ExecuteOnChatSettings()
        {
            view.Hide(false);

            var profEditWnd = new ChatSettingWindow();
            profEditWnd.Owner = (Window)view;
            profEditWnd.ShowDialog();

            view.Hide(true);
        }

        private void ExecuteOnProfileSettings()
        {
            view.Hide(false);

            var profEditWnd = new UserProfileWindow();
            profEditWnd.Owner = (Window)view;
            profEditWnd.ShowDialog();

            view.Hide(true);
        }
    }
}