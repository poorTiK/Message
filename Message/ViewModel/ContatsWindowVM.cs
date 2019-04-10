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
                    try
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
                    }
                    catch (System.Exception)
                    {

                    }
                });
            }
        }

        private string _contactsSearch;

        public string ContactsSearch
        {
            get { return _contactsSearch; }
            set
            {
                try
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
                catch (System.Exception)
                {

                }
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
            try
            {
                view = iview;

                var tempUiInfos = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id).Where(x => x is UserUiInfo).ToList();
                GlobalBase.loadPictures(UserServiceClient, tempUiInfos);
                ContactsList = tempUiInfos;

                IsAddEnabled = false;
                ManageControls();
            }
            catch (System.Exception)
            {

            }
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
            try
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
            catch (System.Exception)
            {

            }
        }

        private void ExecuteOnCloseCommand()
        {
            try
            {
                view.CloseWindow();
            }
            catch (System.Exception)
            {

            }
        }

        private void ExecuteOnAddContactCommand()
        {
            try
            {
                if (SelectedContact != null && SelectedContact is UserUiInfo userUI)
                {
                    var user = UserServiceClient.GetUserById(userUI.UserId);
                    UserServiceClient.AddUserToUserContact(GlobalBase.CurrentUser.Id, user.Id);

                    UpdateContacts();
                }

                ManageControls();
            }
            catch (System.Exception)
            {

            }
        }

        private void UpdateContacts()
        {
            try
            {
                var uiInfos = UserServiceClient.GetAllContactsUiInfo(GlobalBase.CurrentUser.Id);
                GlobalBase.loadPictures(UserServiceClient, uiInfos);

                ContactsList = uiInfos;
                ManageControls();

                GlobalBase.UpdateContactList();
            }
            catch (System.Exception)
            {

            }
        }

        private void ManageControls()
        {
            try
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
            catch (System.Exception)
            {

            }
        }
    }
}