using Message.Interfaces;
using Message.Model;
using Prism.Commands;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Message.PhotoServiceReference;

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

        private Image _image;

        public Image Images
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Images")); }

        }
        private void SetAvatarForUI()
        {
            if (GlobalBase.CurrentUser?.Avatar?.Length > 0)
            {
                MemoryStream memstr = new MemoryStream(GlobalBase.CurrentUser.Avatar);
                Dispatcher.CurrentDispatcher.Invoke(() => { Images = Image.FromStream(memstr); });
            }
            else
            {
                Dispatcher.CurrentDispatcher.Invoke(() => { Images = null; });

            }
        }
        public SettingsWindowVM(IView view)
        {
            this.view = view;
            using (PhotoServiceClient client = new PhotoServiceClient())
            {
                GlobalBase.CurrentUser.Avatar = client.GetPhotoById(GlobalBase.CurrentUser.Id);
            }

            SetAvatarForUI();
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
            GlobalBase.UpdateUI.Invoke();

            view.Hide(true);
        }
    }
}