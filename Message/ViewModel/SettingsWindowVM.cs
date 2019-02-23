using Message.Interfaces;
using Message.Model;

namespace Message.ViewModel
{
    internal class SettingsWindowVM : Prism.Mvvm.BindableBase
    {
        public IView view;

        private string _currentUserName;

        public string CurrentUserName
        {
            get { return GlobalBase.CurrentUser.FirstName + " " + GlobalBase.CurrentUser.LastName; }
            set { SetProperty(ref _currentUserName, value); }
        }

        private string _currentUserLogin;

        public string CurrentUserLogin
        {
            get { return "@" + GlobalBase.CurrentUser.Login; }
            set { SetProperty(ref _currentUserLogin, value); }
        }

        public SettingsWindowVM(IView view)
        {
            this.view = view;
        }
    }
}