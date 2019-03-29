using Message.AdditionalItems;
using Message.Interfaces;
using Message.Compression;
using Message.UserServiceReference;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Message.PhotoServiceReference;
using Message.FileService;
using Message.Model;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class UserProfileWindowVM : BaseViewModel
    {
        private IView _view;
        private byte[] _newAvatar;

        private Image _image;

        public Image Images
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Images")); }

        }

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
            get { return GlobalBase.FileServiceClient.getChatFileById(GlobalBase.CurrentUser.ImageId).Source; }
            set { SetProperty(ref _currentUserPhoto, value); }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set
            {
                SetProperty(ref _userName, value);
                CheckChanges();
            }
        }

        private string _userLastName;

        public string UserLastName
        {
            get { return _userLastName; }
            set
            {
                SetProperty(ref _userLastName, value);
                CheckChanges();
            }
        }

        private string _userPhone;

        public string UserPhone
        {
            get { return _userPhone; }
            set
            {
                SetProperty(ref _userPhone, value);
                CheckChanges();
            }
        }

        private string _userEmail;

        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                SetProperty(ref _userEmail, value);
                CheckChanges();
            }
        }

        private string _userBio;

        public string UserBio
        {
            get { return _userBio; }
            set
            {
                SetProperty(ref _userBio, value);
                CheckChanges();
            }
        }

        private bool _isNewChanges;

        public bool IsNewChanges
        {
            get { return _isNewChanges; }
            set { SetProperty(ref _isNewChanges, value); }
        }

        private bool _isSavingProgress;

        public bool IsSavingProgress
        {
            get { return _isSavingProgress; }
            set { SetProperty(ref _isSavingProgress, value); }
        }

        public UserProfileWindowVM(IView view) : base()
        {
            _view = view;

            UserName = GlobalBase.CurrentUser.FirstName;
            UserLastName = GlobalBase.CurrentUser.LastName;
            UserPhone = GlobalBase.CurrentUser.Phone;
            UserEmail = GlobalBase.CurrentUser.Email;
            UserBio = GlobalBase.CurrentUser.Bio;

            IsNewChanges = false;

            SetAvatarForUI();
        }

        private DelegateCommand _onCloseCommand;

        public DelegateCommand CloseCommand =>
            _onCloseCommand ?? (_onCloseCommand = new DelegateCommand(ExecuteClose));

        private void ExecuteClose()
        {

            GlobalBase.CurrentUser.ImageId = GlobalBase.FileServiceClient.getChatFileById(GlobalBase.CurrentUser.ImageId).Id;   
            _view.CloseWindow();
        }

        private DelegateCommand _onApplyChanges;

        public DelegateCommand ApplyChanges =>
            _onApplyChanges ?? (_onApplyChanges = new DelegateCommand(ExecuteOnApplyChanges));

        private DelegateCommand _onLoadPhoto;

        public DelegateCommand LoadPhoto =>
            _onLoadPhoto ?? (_onLoadPhoto = new DelegateCommand(ExecuteOnLoadPhoto));
        
        private void ExecuteOnLoadPhoto()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            var FilePath = openFileDialog.FileName;

            if (FilePath != string.Empty)
            {
                _newAvatar = File.ReadAllBytes(FilePath);
                MemoryStream memstr = new MemoryStream(_newAvatar);
                Dispatcher.CurrentDispatcher.Invoke(() => { Images = Image.FromStream(memstr); });
                IsNewChanges = true;
            }
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

        private void ExecuteOnApplyChanges()
        {
            IsSavingProgress = true;
            string res;
            Task.Run((() =>
            {
            if (Validate())
            {
                GlobalBase.CurrentUser.FirstName = UserName;
                GlobalBase.CurrentUser.LastName = UserLastName;
                GlobalBase.CurrentUser.Phone = UserPhone;
                GlobalBase.CurrentUser.Email = UserEmail;
                GlobalBase.CurrentUser.Bio = UserBio;
                var chatFile = GlobalBase.FileServiceClient.getChatFileById(GlobalBase.CurrentUser.ImageId);
                res = UserServiceClient.AddOrUpdateUser(GlobalBase.CurrentUser);

                if (_newAvatar != null)
                {
                    if (chatFile == null)
                    {
                            GlobalBase.CurrentUser.ImageId = GlobalBase.FileServiceClient.UploadFile(new FileService.ChatFile() { Source = CompressionHelper.CompressImage(_newAvatar) });
                            UserServiceClient.AddOrUpdateUser(GlobalBase.CurrentUser);
                        }
                        else
                        {
                            GlobalBase.FileServiceClient.UpdateFileSource(chatFile.Id, CompressionHelper.CompressFile(_newAvatar));
                        }
                    }

                    SetAvatarForUI();

                    if (res == string.Empty)
                    {
                        Application.Current.Dispatcher.Invoke(new Action((() =>
                        {
                            CustomMessageBox.Show(Translations.GetTranslation()["ChangesSaved"].ToString());
                        })));
                    }
                }
            })).ContinueWith((task =>
            {
                IsSavingProgress = false;
                IsNewChanges = false;
            }));
        }

        private bool Validate()
        {
            string message = string.Empty;
            if (string.IsNullOrWhiteSpace(UserName))
            {
                message = Application.Current.Resources.MergedDictionaries[4]["FirstNameValid"].ToString();
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UserLastName))
            {
                message = Application.Current.Resources.MergedDictionaries[4]["LastNameValid"].ToString();
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UserEmail) || UserEmail == string.Empty || !Regex.IsMatch(UserEmail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                message = Application.Current.Resources.MergedDictionaries[4]["EmailEmpty"].ToString();
                return false;
            }

            if (message != string.Empty)
            {
                Application.Current.Dispatcher.Invoke(new Action((() => { CustomMessageBox.Show(Application.Current.Resources.MergedDictionaries[4]["Error"].ToString(), message); })));
                return false;
            }

            return true;
        }

        private void CheckChanges()
        {
            if (UserName != GlobalBase.CurrentUser.FirstName || UserLastName != GlobalBase.CurrentUser.LastName ||
                UserPhone != GlobalBase.CurrentUser.Phone ||
                UserEmail != GlobalBase.CurrentUser.Email || UserBio != GlobalBase.CurrentUser.Bio)
            {
                IsNewChanges = true;
            }
        }
    }
}