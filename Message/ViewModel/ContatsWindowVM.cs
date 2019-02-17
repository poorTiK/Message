using Message.ContactsServiceReference;
using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Message.ViewModel
{
    class ContatsWindowVM : Prism.Mvvm.BindableBase
    {
        UserServiceClient UserServiceClient;
        ContactsServiceClient contactsServiceClient;
        IView view;

        private List<UserServiceReference.User> _contacts;
        public List<UserServiceReference.User> ContactsList
        {
            get { return _contacts; }
            set { SetProperty(ref _contacts, value); }
        }

        private UserServiceReference.User _selectedContact;
        public UserServiceReference.User SelectedContact
        {
            get { return _selectedContact; }
            set { SetProperty(ref _selectedContact, value); }
        }

        private string _contactsSearch;
        public string ContactsSearch
        {
            get { return _contactsSearch; }
            set { SetProperty(ref _contactsSearch, value);
                ContactsList = UserServiceClient.GetAllUsersByLogin(ContactsSearch);
            }
        }

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set { SetProperty(ref _searchString, value); }
        }

        public ContatsWindowVM(IView iview)
        {
            view = iview;

            //todo: remove this before release
            //if(GlobalBase.CurrentUser.Login == "admin")
            //{
            //GlobalBase.CurrentUser = new User();
            //GlobalBase.CurrentUser.Id = 2;
            //}

            UserServiceClient = new UserServiceClient();
            contactsServiceClient = new ContactsServiceClient();

            var res = contactsServiceClient.GetContacts(new ContactsServiceReference.User() { Login = GlobalBase.CurrentUser.Login });
            foreach (var item in res)
            {
                ContactsList.Add(new UserServiceReference.User()
                {
                    FirstName = item.UserOwned.FirstName,
                    LastName = item.UserOwned.LastName,
                    LastOnline = item.UserOwned.LastOnline
                });
            }
        }

        private DelegateCommand _onAddContact;
        public DelegateCommand AddContactCommand =>
            _onAddContact ?? (_onAddContact = new DelegateCommand(ExecuteOnAddContactCommand));

        private DelegateCommand _onClose;
        public DelegateCommand CloseCommand =>
            _onClose ?? (_onClose = new DelegateCommand(ExecuteOnCloseCommand));

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
        }

        private void UpdateContacts()
        {
            var res = contactsServiceClient.GetContacts(new ContactsServiceReference.User() { Login = GlobalBase.CurrentUser.Login });
            foreach (var item in res)
            {
                ContactsList.Add(new UserServiceReference.User()
                {
                    FirstName = item.UserOwned.FirstName,
                    LastName = item.UserOwned.LastName,
                    LastOnline = item.UserOwned.LastOnline
                });
            }
        }
    }
}
