using Message.Interfaces;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.ViewModel
{
    class MessageMainVM : Prism.Mvvm.BindableBase
    {
        IView _view;

        public User CurrentUser { get; set; }

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

        public MessageMainVM(IView View)
        {
            _view = View;
        }

        public MessageMainVM(IView View, User user)
        {
            _view = View;
            CurrentUser = user;
        }

        private DelegateCommand _onContactsCommand;
        public DelegateCommand ContactsCommand =>
            _onContactsCommand ?? (_onContactsCommand = new DelegateCommand(ExecuteOnContacts));

        private DelegateCommand _onSettingsCommand;
        public DelegateCommand SettingsCommand =>
            _onSettingsCommand ?? (_onSettingsCommand = new DelegateCommand(ExecuteOnSettingsCommand));

        private void ExecuteOnSettingsCommand()
        {
            //throw new NotImplementedException();
        }

        private void ExecuteOnContacts()
        {
            var wnd = new Contacts();
            wnd.ShowDialog();
        }
    }
}
