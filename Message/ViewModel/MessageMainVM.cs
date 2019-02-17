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
using Message.AdditionalItems;

namespace Message.ViewModel
{
    class MessageMainVM : Prism.Mvvm.BindableBase
    {
        UserServiceClient userServiceClient;

        IView _view;

        public UserServiceReference.User CurrentUser { get; set; }
        
        private string _currentUserName;
        public string CurrentUserName
        {
            get { return CurrentUser.FirstName + " " + CurrentUser.LastName; }
            set { SetProperty(ref _currentUserName, value); }
        }

        private string _currentUserLogin;
        public string CurrentUserLogin
        {
            get { return "@" + CurrentUser.Login; }
            set { SetProperty(ref _currentUserLogin, value); }
        }

        private bool _isDialogSearchVisible;
        public bool IsDialogSearchVisible
        {
            get { return _isDialogSearchVisible; }
            set { SetProperty(ref _isDialogSearchVisible, value); }
        }

        private List<UserServiceReference.User> _contactsList;
        public List<UserServiceReference.User> ContactsList
        {
            get { return _contactsList; }
            set { SetProperty(ref _contactsList, value); }
        }

        public MessageMainVM(IView View)
        {
            _view = View;

            userServiceClient = new UserServiceClient();
        }

        public MessageMainVM(IView View, UserServiceReference.User user)
        {
            _view = View;
            CurrentUser = user;
            GlobalBase.CurrentUser = user;

            userServiceClient = new UserServiceClient();

            ContactsList = userServiceClient.GetAllContacts(GlobalBase.CurrentUser);
        }

        private DelegateCommand _onContactsCommand;
        public DelegateCommand ContactsCommand =>
            _onContactsCommand ?? (_onContactsCommand = new DelegateCommand(ExecuteOnContacts));

        private DelegateCommand _onSettingsCommand;
        public DelegateCommand SettingsCommand =>
            _onSettingsCommand ?? (_onSettingsCommand = new DelegateCommand(ExecuteOnSettingsCommand));

        private DelegateCommand _dialogSearchCommand;
        public DelegateCommand DialogSearchCommand =>
            _dialogSearchCommand ?? (_dialogSearchCommand = new DelegateCommand(() => { IsDialogSearchVisible = !IsDialogSearchVisible; }));


        private void ExecuteOnSettingsCommand()
        {
            _view.SetOpacity(0.5);
            
            var wnd = new SettingsWindow();
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();

            Update();

            _view.SetOpacity(1);
        }

        private void ExecuteOnContacts()
        {
            _view.SetOpacity(0.5);

            var wnd = new Contacts();
            wnd.Owner = (Window)_view;
            wnd.ShowDialog();

            _view.SetOpacity(1);
        }

        public void Update()
        {
            ContactsList = userServiceClient.GetAllContacts(GlobalBase.CurrentUser);
            //var res = contactsServiceClient.GetContacts(new ContactsServiceReference.User() { Login = GlobalBase.CurrentUser.Login });
            //foreach (var item in res)
            //{
            //    ContactsList.Add(new UserServiceReference.User()
            //    {
            //        FirstName = item.UserOwned.FirstName,
            //        LastName = item.UserOwned.LastName,
            //        LastOnline = item.UserOwned.LastOnline
            //    });
            //}
        }
    }
}
