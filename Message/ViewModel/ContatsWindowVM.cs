using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;

namespace Message.ViewModel
{
    internal class ContatsWindowVM : Prism.Mvvm.BindableBase
    {
        private UserServiceClient UserServiceClient;
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
                    ContactsList = UserServiceClient.GetAllUsersByLogin(ContactsSearch);
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

            UserServiceClient = new UserServiceClient();

            ContactsList = UserServiceClient.GetAllContacts(GlobalBase.CurrentUser);

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
            throw new NotImplementedException();
        }

        private void ExecuteOnCloseCommand()
        {
            view.CloseWindow();
        }

        private void ExecuteOnAddContactCommand()
        {
            if (SelectedContact != null)
            {
                UserServiceClient.AddContact(GlobalBase.CurrentUser, SelectedContact);
                UpdateContacts();
            }

            ManageControls();
        }

        private void UpdateContacts()
        {
            ContactsList = UserServiceClient.GetAllContacts(GlobalBase.CurrentUser);

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
    }
}