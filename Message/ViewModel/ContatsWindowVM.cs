using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Threading;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class ContatsWindowVM : BaseViewModel
    {
        private IView view;

        private List<UiInfo> _contacts;

        public List<UiInfo> ContactsList
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

        private UiInfo _selectedContact;

        public UiInfo SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                SetProperty(ref _selectedContact, value, () =>
                {
                    if (value != null)
                    {
                        if (!UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id).Where(x => x is UserUiInfo).Any(x => x.UniqueName == value.UniqueName))
                        {
                            IsAddEnabled = true;
                            return;
                        }
                    }
                    IsAddEnabled = false;
                });
            }
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
                    var tempUiInfos = UserServiceClient.FindNewUsersUiUnfoByLogin(GlobalBase.CurrentUser.Id, ContactsSearch);
                    GlobalBase.loadPictures(UserServiceClient, tempUiInfos);
                    ContactsList = tempUiInfos;
                }
                else
                {
                    UpdateContacts();
                }

                ManageControls();
            }
        }

        private bool _isAddEnabled;

        public bool IsAddEnabled
        {
            get { return _isAddEnabled; }
            set { SetProperty(ref _isAddEnabled, value); }
        }

        public ContatsWindowVM(IView iview) : base()
        {
            view = iview;

            var tempUiInfos = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id).Where(x => x is UserUiInfo).ToList();
            GlobalBase.loadPictures(UserServiceClient, tempUiInfos);
            ContactsList = tempUiInfos;

            IsAddEnabled = false;
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
            if (SelectedContact is UserUiInfo userUI)
            {
                var user = UserServiceClient.GetUserById(userUI.UserId);

                var wnd = new ContactProfileWindow(user);
                wnd.Owner = (Window)view;
                wnd.ShowDialog();

                UpdateContacts();
            }
        }

        private void ExecuteOnCloseCommand()
        {
            view.CloseWindow();
        }

        private void ExecuteOnAddContactCommand()
        {
            if (SelectedContact != null && SelectedContact is UserUiInfo userUI)
            {
                var user = UserServiceClient.GetUserById(userUI.UserId);
                UserServiceClient.AddUserToUserContact(GlobalBase.CurrentUser.Id, user.Id);

                UpdateContacts();
            }

            ManageControls();
        }

        private void UpdateContacts()
        {
            var uiInfos = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);
            GlobalBase.loadPictures(UserServiceClient, uiInfos);

            ContactsList = uiInfos;
            ManageControls();

            GlobalBase.UpdateContactList();
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