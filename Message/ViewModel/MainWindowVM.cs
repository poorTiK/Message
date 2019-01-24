using Message.Interfaces;
using Message.ServiceReference1;
using Prism.Commands;
using System;
using System.Net;

namespace Message.ViewModel
{
    class MainWindowVM : Prism.Mvvm.BindableBase
    {
        IView view;
        IPasswordSupplier passwordSupplier;

        ServerClient proxy;
        const string HOST = "localhost";

        private DelegateCommand _onStartRegister;
        public DelegateCommand OnStartRegister =>
            _onStartRegister ?? (_onStartRegister = new DelegateCommand(ExecuteOnStartRegister));

        private DelegateCommand _onLogin;
        public DelegateCommand OnLogin =>
            _onLogin ?? (_onLogin = new DelegateCommand(ExecuteOnLogin));

        private DelegateCommand _onForgotPassword;
        public DelegateCommand OnForgotPassword =>
            _onForgotPassword ?? (_onForgotPassword = new DelegateCommand(ExecuteOnForgotPassword));

        private DelegateCommand _register;
        public DelegateCommand Register =>
            _register ?? (_register = new DelegateCommand(ExecuteOnRegister));
        
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

        #region Login data
        private string _loginText;
        public string LoginText
        {
            get { return _loginText; }
            set { SetProperty(ref _loginText, value); }
        }

        private string _password;
        public string Password
        {
            get {
                return passwordSupplier.GetPasswordForLogin();
            }
            set { SetProperty(ref _password, value); }
        }

        #endregion

        #region registration data
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _surname;
        public string Surname
        {
            get { return _surname; }
            set { SetProperty(ref _surname, value); }
        }

        private string _userLogin;
        public string UserLogin
        {
            get { return _userLogin; }
            set { SetProperty(ref _userLogin, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private string _RPassword;
        public string RPassword
        {
            get { return passwordSupplier.GetPasswordForRegistration(); }
            set { SetProperty(ref _RPassword, value); }
        }

        private string _Rep_RPassword;
        public string Rep_RPassword
        {
            get { return passwordSupplier.GetRepPasswordForRegistration(); }
            set { SetProperty(ref _Rep_RPassword, value); }
        }

        #endregion

        public MainWindowVM(IView iView, IPasswordSupplier ipasswordSupplier)
        {
            view = iView;
            passwordSupplier = ipasswordSupplier;

            proxy = new ServerClient();

            IsSignUpVisible = true;
            IsRegisterVisible = false;
        }

        void ExecuteOnStartRegister()
        {
            if (IsSignUpVisible) {
                view.AnimatedResize(450, 310);
                IsSignUpVisible = false;
                IsRegisterVisible = true;
            } else {
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
            var user = new User()
            {
                Login = "test",
                Password = "test",
                FirstName = "test",
                Email = "test",
                LastOnline = new DateTime(2015, 12, 12)
            };

            proxy.AddNewUser(user);
            
            MessageMainWnd wnd = new MessageMainWnd();
            wnd.Show();
            view.CloseWindow();
        }

        private void ExecuteOnRegister()
        {
            if (!string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(UserLogin) && !string.IsNullOrWhiteSpace(RPassword) && !string.IsNullOrWhiteSpace(Rep_RPassword) 
                && !string.IsNullOrWhiteSpace(Email) && (RPassword == Rep_RPassword))
            {
                var user = new User()
                {
                    Login = UserLogin,
                    Password = RPassword,
                    FirstName = Name,
                    Email = Email,
                    LastOnline = new DateTime(2015, 12, 12)
                    
                };

                if (proxy.AddNewUser(user))
                {
                    //mesage "Succesfull registration"
                    ExecuteOnBackCommand();
                }
            }
            else
            {
                //mesage box "You enter wrong data"
            }
        }

        private void ExecuteOnForgotPassword()
        {
            throw new NotImplementedException();
        }
    }
}
