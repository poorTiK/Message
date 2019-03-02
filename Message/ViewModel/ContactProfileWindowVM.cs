using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.ServiceModel;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class ContactProfileWindowVM : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        private IView _view;

        private InstanceContext usersSite;
        private UserServiceClient UserServiceClient;
        private IUserServiceCallback _userServiceCallback;

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
            UserServiceClient = new UserServiceClient(usersSite);

            UserName = Profile.FirstName;
            UserLastName = Profile.LastName;
            UserPhone = Profile.Phone;
            UserEmail = Profile.Email;
            UserBio = Profile.Bio;

            var contact = UserServiceClient.IsExistsInContacts(GlobalBase.CurrentUser, Profile);

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
        }

        private DelegateCommand _onAddContact;

        public DelegateCommand AddContact =>
            _onAddContact ?? (_onAddContact = new DelegateCommand(ExecuteOnAddContact));

        private DelegateCommand _onDeleteContact;

        public DelegateCommand DeleteContact =>
            _onDeleteContact ?? (_onDeleteContact = new DelegateCommand(ExecuteOnDeleteContact));

        private void ExecuteOnAddContact()
        {
            UserServiceClient.AddContactAsync(GlobalBase.CurrentUser, Profile);
            ManageControls();
        }

        private void ExecuteOnDeleteContact()
        {
            UserServiceClient.RemoveContactAsync(GlobalBase.CurrentUser, Profile);
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
    }
}