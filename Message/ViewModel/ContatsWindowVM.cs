using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using Message.PhotoServiceReference;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class ContatsWindowVM : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        private InstanceContext usersSite;
        private UserServiceClient UserServiceClient;
        private IUserServiceCallback _userServiceCallback;

        private IView view;

        private List<User> _contacts;

        public List<User> ContactsList
        {
            get { return _contacts; }
            set { SetProperty(ref _contacts, value); }
        }

        private string _caption;

        public string Caption
        {
            get { return _caption; }
            set { SetProperty(ref _caption, value); }
        }

        private User _selectedContact;

        public User SelectedContact
        {
            get { return _selectedContact; }
            set { SetProperty(ref _selectedContact, value); }
        }

        private string _contactsSearch;

        public string ContactsSearch
        {
            get { return _contactsSearch; }
            set
            {
                SetProperty(ref _contactsSearch, value);

                if (!string.IsNullOrEmpty(value))
                {
                    using (var proxy = new UserServiceClient(usersSite))
                    {
                        ContactsList = proxy.FindUsersByLogin(ContactsSearch);
                    }
                }
                else
                {
                    UpdateContacts();
                }

                ManageControls();
            }
        }

        public ContatsWindowVM(IView iview)
        {
            view = iview;

            _userServiceCallback = this;
            usersSite = new InstanceContext(_userServiceCallback);
            using (UserServiceClient = new UserServiceClient(usersSite))
            {
                ContactsList = UserServiceClient.GetAllContacts(GlobalBase.CurrentUser.Id);
            }

            using (var proxy = new PhotoServiceClient())
            {
                foreach (var item in ContactsList)
                {
                    item.Avatar = proxy.GetPhotoById(item.Id);
                }
            }

            ManageControls();
        }

        private DelegateCommand _onAddContact;

        public DelegateCommand AddContactCommand =>
            _onAddContact ?? (_onAddContact = new DelegateCommand(ExecuteOnAddContactCommand));

        private DelegateCommand _onClose;

        public DelegateCommand CloseCommand =>
            _onClose ?? (_onClose = new DelegateCommand(ExecuteOnCloseCommand));

        private DelegateCommand _openProfile;

        public DelegateCommand OpenProfile =>
            _openProfile ?? (_openProfile = new DelegateCommand(ExecuteOnOpenProfile));

        private void ExecuteOnOpenProfile()
        {
            var wnd = new ContactProfileWindow(SelectedContact);
            wnd.Owner = (Window)view;
            wnd.ShowDialog();

            UpdateContacts();
        }

        private void ExecuteOnCloseCommand()
        {
            view.CloseWindow();
        }

        private void ExecuteOnAddContactCommand()
        {
            if (SelectedContact != null)
            {
                using (var proxy = new UserServiceClient(usersSite))
                {
                    proxy.AddContact(GlobalBase.CurrentUser.Id, SelectedContact.Id);
                    UpdateContacts();
                }
            }

            ManageControls();
        }

        private void UpdateContacts()
        {
            using (var proxy = new UserServiceClient(usersSite))
            {
                ContactsList = proxy.GetAllContacts(GlobalBase.CurrentUser.Id);
            }
            using (var proxy = new PhotoServiceClient())
            {
                foreach (var item in ContactsList)
                {
                    item.Avatar = proxy.GetPhotoById(item.Id);
                }
            }

            ManageControls();
        }

        private void ManageControls()
        {
            if (string.IsNullOrEmpty(ContactsSearch))
            {
                Caption = "Contacts";
            }
            else
            {
                Caption = "Contacts search";
            }
        }

        public void UserLeave(User user)
        {
            //int test = 10;
        }

        public void UserCame(User user)
        {
            //int test = 20;
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