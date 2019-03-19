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
using Message.FileService;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class ContactProfileWindowVM : BaseViewModel
    {
        private IView _view;
        private Image _image;

        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Image"));
            }
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
            get { return GlobalBase.FileServiceClient.getChatFileById(Profile.ImageId).Source; }
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

        public ContactProfileWindowVM(IView view, User user) : base()
        {
            _view = view;
            Profile = user;

            UserName = Profile.FirstName;
            UserLastName = Profile.LastName;
            UserPhone = Profile.Phone;
            UserEmail = Profile.Email;
            UserBio = Profile.Bio;
            bool contact;

            contact = UserServiceClient.IsExistsInContacts(GlobalBase.CurrentUser.Id, Profile.Id);           

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
            UserServiceClient.AddUserToUserContact(GlobalBase.CurrentUser.Id, Profile.Id);
            ManageControls();
        }

        private void ExecuteOnDeleteContact()
        {
            UserServiceClient.RemoveUserToUserContact(GlobalBase.CurrentUser.Id, Profile.Id);
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
            if (Profile.ImageId != 0)
            {
                ChatFile chatFile = GlobalBase.FileServiceClient.getChatFileById(Profile.ImageId);

                if (chatFile?.Source?.Length > 0)
                {
                    MemoryStream memstr = new MemoryStream(chatFile.Source);
                    Dispatcher.CurrentDispatcher.Invoke(() => { Image = Image.FromStream(memstr); });
                }
                else
                {
                    Dispatcher.CurrentDispatcher.Invoke(() => { Image = Image.FromFile(@"../../Resources/DefaultPicture.jpg"); });
                }
            }
        }
    }
}