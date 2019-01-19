using Message.Interfaces;
using Message.ServiceReference1;
using Prism.Commands;
using System.Net;

namespace Message.ViewModel
{
    class MainWindowVM : Prism.Mvvm.BindableBase
    {
        IView view;

        ServerClient proxy;
        const string HOST = "localhost";

        private DelegateCommand _onStartRegister;
        public DelegateCommand OnStartRegister =>
            _onStartRegister ?? (_onStartRegister = new DelegateCommand(ExecuteOnStartRegister));

        private DelegateCommand _onLogin;
        public DelegateCommand OnLogin =>
            _onLogin ?? (_onLogin = new DelegateCommand(ExecuteOnLogin));

        private DelegateCommand _onBackCommand;
        public DelegateCommand BackCommand =>
            _onBackCommand ?? (_onBackCommand = new DelegateCommand(ExecuteOnBackCommand));

        private bool _isSignUpVisible;
        public bool IsSignUpVisible
        {
            get { return _isSignUpVisible; }
            set { SetProperty(ref _isSignUpVisible, value); }
        }

        private bool _isRegisterVisible;
        public bool IsRegisterVisible
        {
            get { return _isRegisterVisible; }
            set { SetProperty(ref _isRegisterVisible, value); }
        }

        #region registration data
        private string fieldName;
        public string PropertyName
        {
            get { return fieldName; }
            set { SetProperty(ref fieldName, value); }
        }
        #endregion

        public MainWindowVM(IView iView)
        {
            view = iView;

            proxy = new ServerClient();

            IsSignUpVisible = true;
            IsRegisterVisible = false;
        }

        void ExecuteOnStartRegister()
        {
            if (IsSignUpVisible)
            {
                view.AnimatedResize(380, 350);
                IsSignUpVisible = false;
                IsRegisterVisible = true;
            }
            else
            {
                view.AnimatedResize(250, 450);
                IsSignUpVisible = true;
                IsRegisterVisible = false;
            }

        }


        private void ExecuteOnBackCommand()
        {
            if (IsSignUpVisible)
            {
                view.AnimatedResize(400, 250);
                IsSignUpVisible = false;
                IsRegisterVisible = true;
            }
            else
            {
                view.AnimatedResize(250, 450);
                IsSignUpVisible = true;
                IsRegisterVisible = false;
            }
        }

        void ExecuteOnLogin()
        {

            //proxy.AddNewUser(new User());
            
            MessageMainWnd wnd = new MessageMainWnd();
            wnd.Show();
            view.CloseWindow();
        }
    }
}
