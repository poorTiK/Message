using Message.AdditionalItems;
using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class UserProfileWindowVM : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        private IView _view;

        private InstanceContext usersSite;
        private UserServiceClient UserServiceClient;
        private IUserServiceCallback _userServiceCallback;

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

        public UserProfileWindowVM(IView view)
        {
            _view = view;
            _userServiceCallback = this;
            usersSite = new InstanceContext(_userServiceCallback);
            UserServiceClient = new UserServiceClient(usersSite);

            UserName = GlobalBase.CurrentUser.FirstName;
            UserLastName = GlobalBase.CurrentUser.LastName;
            UserPhone = GlobalBase.CurrentUser.Phone;
            UserEmail = GlobalBase.CurrentUser.Email;
            UserBio = GlobalBase.CurrentUser.Bio;

            IsNewChanges = false;
        }

        private DelegateCommand _onApplyChanges;

        public DelegateCommand ApplyChanges =>
            _onApplyChanges ?? (_onApplyChanges = new DelegateCommand(ExecuteOnApplyChanges));

        private void ExecuteOnApplyChanges()
        {
            IsSavingProgress = true;
            bool res;
            Task.Run((() =>
            {
                if (Validate())
                {
                    GlobalBase.CurrentUser.FirstName = UserName;
                    GlobalBase.CurrentUser.LastName = UserLastName;
                    GlobalBase.CurrentUser.Phone = UserPhone;
                    GlobalBase.CurrentUser.Email = UserEmail;
                    GlobalBase.CurrentUser.Bio = UserBio;

                    res = UserServiceClient.AddOrUpdateUser(GlobalBase.CurrentUser);

                    if (res)
                    {
                        Application.Current.Dispatcher.Invoke(new Action((() =>
                        {
                            CustomMessageBox.Show("Changes saved");
                        })));
                    }
                }
            })).ContinueWith((task => { IsSavingProgress = false; }));
        }

        private bool Validate()
        {
            string message = string.Empty;
            if (string.IsNullOrWhiteSpace(UserName))
            {
                message = "First name can't be empty";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UserLastName))
            {
                message = "Last name can't be empty";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UserEmail) || UserEmail == string.Empty || !Regex.IsMatch(UserEmail, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                message = "Email can't be empty\n and must be valid";
                return false;
            }

            if (message != string.Empty)
            {
                Application.Current.Dispatcher.Invoke(new Action((() => { CustomMessageBox.Show("Error", message); })));
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

        public void UserLeave(User user)
        {
            throw new NotImplementedException();
        }

        public void UserCame(User user)
        {
            throw new NotImplementedException();
        }
    }
}