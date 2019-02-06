using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.ViewModel
{
    class ContatsWindowVM : Prism.Mvvm.BindableBase
    {
        UserServiceClient UserServiceClient;

        IView view;

        private List<User> _contacts;
        public List<User> ContactsList
        {
            get { return _contacts; }
            set { SetProperty(ref _contacts, value); }
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

            UserServiceClient = new UserServiceClient();

            ContactsList = UserServiceClient.GetAllContacts(GlobalBase.CurrentUser);
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
                //UpdateContacts();
            }
        }

        private void UpdateContacts()
        {
            ContactsList = UserServiceClient.GetAllContacts(GlobalBase.CurrentUser);
        }
    }
}
