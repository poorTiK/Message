using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Windows.Threading;
using Message.PhotoServiceReference;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class ContactProfileWindowVM : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        private IView _view;

        private InstanceContext usersSite;
        private UserServiceClient UserServiceClient;
        private IUserServiceCallback _userServiceCallback;
        private Image _image;

        public Image Images
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Images")); }

        }
        private User Profile;

        private string _currentUserName;

        public string CurrentUserName
        {
            get { return Profile.FirstName + " " + Profile.LastName; }
            set { SetProperty(ref _currentUserName, value); }
        }

        private string _currentUserLogin;

        public string CurrentUserLogin
        {
            get { return "@" + Profile.Login; }
            set { SetProperty(ref _currentUserLogin, value); }
        }

        private byte[] _currentUserPhoto;

        public byte[] CurrentUserPhoto
        {
            get { return Profile.Avatar; }
            set { SetProperty(ref _currentUserPhoto, value); }
        }

        private string _userName;

        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _userLastName;

        public string UserLastName
        {
            get { return _userLastName; }
            set { SetProperty(ref _userLastName, value); }
        }

        private string _userPhone;

        public string UserPhone
        {
            get { return _userPhone; }
            set
            {
                SetProperty(ref _userPhone, value);
            }
        }

        private string _userEmail;

        public string UserEmail
        {
            get { return _userEmail; }
            set { SetProperty(ref _userEmail, value); }
        }

        private string _userBio;

        public string UserBio
        {
            get { return _userBio; }
            set { SetProperty(ref _userBio, value); }
        }

        private bool _isContact;

        public bool IsContact
        {
            get { return _isContact; }
            set { SetProperty(ref _isContact, value); }
        }

        private bool _isNonContact;

        public bool IsNonContact
        {
            get { return _isNonContact; }
            set { SetProperty(ref _isNonContact, value); }
        }

        public ContactProfileWindowVM(IView view, User user)
        {
            _view = view;
            Profile = user;

            _userServiceCallback = this;
            usersSite = new InstanceContext(_userServiceCallback);

            UserName = Profile.FirstName;
            UserLastName = Profile.LastName;
            UserPhone = Profile.Phone;
            UserEmail = Profile.Email;
            UserBio = Profile.Bio;
            bool contact;

            using (UserServiceClient = new UserServiceClient(usersSite))
            {
                 contact = UserServiceClient.IsExistsInContacts(GlobalBase.CurrentUser.Id, Profile.Id);           
            }

            if (contact)
            {
                IsContact = true;
                IsNonContact = false;
            }
            else
            {
                IsContact = false;
                IsNonContact = true;
            }

            SetAvatarForUI();
        }

        private DelegateCommand _onAddContact;

        public DelegateCommand AddContact =>
            _onAddContact ?? (_onAddContact = new DelegateCommand(ExecuteOnAddContact));

        private DelegateCommand _onDeleteContact;

        public DelegateCommand DeleteContact =>
            _onDeleteContact ?? (_onDeleteContact = new DelegateCommand(ExecuteOnDeleteContact));

        private void ExecuteOnAddContact()
        {
            using (UserServiceClient = new UserServiceClient(usersSite))
            {
                UserServiceClient.AddContactAsync(GlobalBase.CurrentUser.Id, Profile.Id);
            }

            ManageControls();
        }

        private void ExecuteOnDeleteContact()
        {
            using (UserServiceClient = new UserServiceClient(usersSite))
            {
                UserServiceClient.RemoveContactAsync(GlobalBase.CurrentUser.Id, Profile.Id);
            }

            ManageControls();
        }

        private void ManageControls()
        {
            if (IsContact)
            {
                IsContact = !IsContact;
                IsNonContact = !IsNonContact;
            }
            else
            {
                IsContact = !IsContact;
                IsNonContact = !IsNonContact;
            }
        }


        private void SetAvatarForUI()
        {
            using (var proxy = new PhotoServiceClient())
            {
                Profile.Avatar = proxy.GetPhotoById(Profile.Id);

            }
            if (Profile?.Avatar?.Length > 0)
            {
                MemoryStream memstr = new MemoryStream(GlobalBase.CurrentUser.Avatar);
                Dispatcher.CurrentDispatcher.Invoke(() => { Images = Image.FromStream(memstr); });
            }
            else
            {
                Dispatcher.CurrentDispatcher.Invoke(() => { Images = null; });

            }

           
        }

        public void UserCame(User user)
        {
            //throw new NotImplementedException();
        }

        public void UserLeave(User user)
        {
            //throw new NotImplementedException();
        }

        public void ReceiveMessage(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public void OnMessageRemoved(BaseMessage message)
        {
            //throw new NotImplementedException();
        }

        public void OnMessageEdited(BaseMessage message)
        {
            //throw new NotImplementedException();
        }
    }
}