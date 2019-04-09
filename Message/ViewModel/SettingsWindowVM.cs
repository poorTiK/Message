﻿using Message.Interfaces;
using Message.Model;
using Prism.Commands;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Message.ViewModel
{
    internal class SettingsWindowVM : BaseViewModel
    {
        private readonly ISerializeUser _serializeUser;

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

        private Image _image;

        public Image Images
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Images")); }
        }

        public SettingsWindowVM(IView view) : base()
        {
            this.view = view;

            _serializeUser = new SerializeUserToRegistry();

            SetAvatarForUI();
        }

        private DelegateCommand _onProfileSettings;

        public DelegateCommand ProfileSettings =>
            _onProfileSettings ?? (_onProfileSettings = new DelegateCommand(ExecuteOnProfileSettings));

        private DelegateCommand _chatSettings;

        public DelegateCommand ChatSettings =>
            _chatSettings ?? (_chatSettings = new DelegateCommand(ExecuteOnChatSettings));

        private DelegateCommand _exitChat;

        public DelegateCommand ExitChat =>
            _exitChat ?? (_exitChat = new DelegateCommand(ExecuteOnExitChat));

        private void ExecuteOnChatSettings()
        {
            view.Hide(false);

            var profEditWnd = new ChatSettingWindow();
            profEditWnd.Owner = (Window)view;
            profEditWnd.ShowDialog();

            view.Hide(true);
        }

        private void UpdateUI()
        {
            SetAvatarForUI();
        }

        private void ExecuteOnProfileSettings()
        {
            view.Hide(false);

            var profEditWnd = new UserProfileWindow();
            profEditWnd.Owner = (Window)view;
            profEditWnd.ShowDialog();
            UpdateUI();
            GlobalBase.UpdateContactList.Invoke();

            view.Hide(true);
        }

        private void ExecuteOnExitChat()
        {
            _serializeUser.CleanCurrentUser();
            GlobalBase.ExitProgramm();
        }

        private void SetAvatarForUI()
        {
            Task.Run(() =>
           {
               Dispatcher.CurrentDispatcher.Invoke(() =>
               {
                   Images = GlobalBase.getUsersAvatar(GlobalBase.CurrentUser);
               });
           });
        }
    }
}