using Message.AdditionalItems;
using Message.Interfaces;
using Message.ApplicationSettingsServiceReference;
using Message.MessageTServiceReference;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

namespace Message.ViewModel
{
    class MainWindowVM : Prism.Mvvm.BindableBase
    {
        IView view;
        IPasswordSupplier passwordSupplier;

        UserServiceClient UserServiceClient;
        ApplicationSettingsServiceClient ApplicationSettingsService;
        MessageTServiceClient MessageTServiceClient;
        

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

            UserServiceClient = new UserServiceClient();
            MessageTServiceClient = new MessageTServiceClient();
            ApplicationSettingsService = new ApplicationSettingsServiceClient();

            IsSignUpVisible = true;
            IsRegisterVisible = false;

            TestApplicationSettings();
            TestMessageT();
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
            if(LoginText == "admin")
            {
                MessageMainWnd wnd = new MessageMainWnd();
                wnd.Show();

                view.CloseWindow();
                return;
            }
            else if (ValidateOnLogin())
            {
                if(UserServiceClient.GetUser(LoginText, Password) != null)
                {
                    MessageMainWnd wnd = new MessageMainWnd();
                    wnd.Show();

                    view.CloseWindow();
                }
            }
        }

        private void ExecuteOnRegister()
        {
            if (ValidateOnRegister())
            {
                var user = new User()
                {
                    Login = UserLogin,
                    Password = RPassword,
                    FirstName = Name,
                    LastName = Surname,
                    Email = Email,
                    LastOnline = DateTime.Now.Date
                };

                if (UserServiceClient.GetUser(UserLogin, RPassword) == null)
                {
                    if (UserServiceClient.AddNewUser(user))
                    {
                        CustomMessageBox.Show("Registration done");
                        ExecuteOnBackCommand();
                    }
                    else
                    {
                        CustomMessageBox.Show("Error!!!", "Registration error");
                    }
                }
                else
                {
                    CustomMessageBox.Show("Error!!!", "Same user exists");
                }
            }
        }

        private void ExecuteOnForgotPassword()
        {
            //throw new NotImplementedException();
        }

        private bool ValidateOnRegister()
        {
            if (string.IsNullOrWhiteSpace(UserLogin))
            {
                CustomMessageBox.Show("Login is empty!!!");
                return false;
            }
            else if (RPassword.Length < 8 || string.IsNullOrWhiteSpace(RPassword) || RPassword == string.Empty || !Regex.IsMatch(RPassword, @"^[a-zA-Z0-9]{8,}$"))
            {
                CustomMessageBox.Show("Error!", "Password shoud be 8 symbols lenght, use numbers and english symbols");
                return false;
            }
            else if (RPassword != Rep_RPassword)
            {
                CustomMessageBox.Show("Error!", "Password not match");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(Email) || Email == string.Empty || !Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                CustomMessageBox.Show("Error!", "Wrong Email");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ValidateOnLogin()
        {
            if (string.IsNullOrWhiteSpace(LoginText))
            {
                CustomMessageBox.Show("Login is empty!!!");
                return false;
            }
            else if (Password.Length < 8 || string.IsNullOrWhiteSpace(Password) || Password == string.Empty || !Regex.IsMatch(Password, @"^[a-zA-Z0-9]{8,}$"))
            {
                CustomMessageBox.Show("Error!", "Password shoud be 8 symbols lenght, use numbers and english symbols");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void TestApplicationSettings()
        {
            //MessageBox.Show(ApplicationSettingsService.Test().ToString());
        }
        private void TestMessageT()
        {
            //MessageBox.Show(MessageTServiceClient.Test().ToString());
        }
    }
}
