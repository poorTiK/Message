using Message.AdditionalItems;
using Message.Interfaces;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class MainWindowVM : Prism.Mvvm.BindableBase, IUserServiceCallback
    {
        private IView view;
        private IPasswordSupplier passwordSupplier;

        private InstanceContext usersSite;
        private UserServiceClient UserServiceClient;
        private IUserServiceCallback _userServiceCallback;

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

        private bool _isLoginProgress;

        public bool IsLoginProgress
        {
            get { return _isLoginProgress; }
            set { SetProperty(ref _isLoginProgress, value); }
        }

        private bool _isRegisterProgress;

        public bool IsRegisterProgress
        {
            get { return _isRegisterProgress; }
            set { SetProperty(ref _isRegisterProgress, value); }
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
            get
            {
                return passwordSupplier.GetPasswordForLogin();
            }
            set { SetProperty(ref _password, value); }
        }

        #endregion Login data

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
            set
            {
                SetProperty(ref _RPassword, value);
                passwordSupplier.ClearPassword();
            }
        }

        private string _Rep_RPassword;

        public string Rep_RPassword
        {
            get { return passwordSupplier.GetRepPasswordForRegistration(); }
            set
            {
                SetProperty(ref _Rep_RPassword, value);
                passwordSupplier.ClearPassword();
            }
        }

        #endregion registration data

        public MainWindowVM(IView iView, IPasswordSupplier ipasswordSupplier)
        {
            view = iView;
            passwordSupplier = ipasswordSupplier;


            //callback for user
            _userServiceCallback = this;
            usersSite = new InstanceContext(_userServiceCallback);
            UserServiceClient = new UserServiceClient(usersSite);

            IsLoginProgress = false;
            IsRegisterProgress = false;

            IsSignUpVisible = true;
            IsRegisterVisible = false;

            //fillUsers();
            //TestApplicationSettings();
            //TestMessageT();
        }

        private void fillUsers()
        {
            User admin = new User()
            {
                Email = "admin@mail.ru",
                Login = "admin",
                Password = "123123",
                FirstName = "Admin",
                LastName = "Purdik",
                Status = "online"
            };

            User steveOwned = new User()
            {
                Email = "firstOwned@mail.ru",
                Login = "firstOwned",
                Password = "123123",
                FirstName = "Steve",
                LastName = "Jobs",
                Status = "online"
            };

            User billOwned = new User()
            {
                Email = "secondOwned@mail.ru",
                Login = "secondOwned",
                Password = "123123",
                FirstName = "Bill",
                LastName = "Gates",
                Status = "online"
            };

            User markOwner = new User()
            {
                Email = "owner@mail.ru",
                Login = "owner",
                Password = "123123",
                FirstName = "Mark",
                LastName = "Zuckerberg",
                Status = "online"
            };

            UserServiceClient.AddOrUpdateUserAsync(markOwner);
            UserServiceClient.AddOrUpdateUserAsync(billOwned);

            //attaching new contact for Mark
            UserServiceClient.AddContactAsync(markOwner, billOwned);

            //just another user in db
            UserServiceClient.AddOrUpdateUserAsync(steveOwned);
        }

        private void ExecuteOnStartRegister()
        {
            if (IsSignUpVisible)
            {
                view.AnimatedResize(450, 310);
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

        private void ExecuteOnLogin()
        {
            IsLoginProgress = true;
            Task.Run(() =>
            {
                if (ValidateOnLogin())
                {
                    var user = UserServiceClient.GetUser(LoginText, Password);
                    if (user != null)
                    {
                        Application.Current.Dispatcher.Invoke(new Action((() =>
                        {
                            MessageMainWnd wnd = new MessageMainWnd(user);
                            wnd.Show();

                            view.CloseWindow();
                        })));
                    }
                }
            }).ContinueWith(task => { IsLoginProgress = false; });
        }

        private void ExecuteOnRegister()
        {
            IsRegisterProgress = true;
            Task.Run(() =>
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
                        Status = "online"
                    };

                    if (UserServiceClient.GetUserByLogin(UserLogin) == null) //if time move validation parts to ValidateOnRegister()
                    {
                        if (UserServiceClient.AddOrUpdateUser(user))
                        {
                            Application.Current.Dispatcher.Invoke(new Action((() =>
                            {
                                CustomMessageBox.Show("Registration done");
                                Clear();
                                ExecuteOnBackCommand();
                            })));
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(new Action((() =>
                            {
                                CustomMessageBox.Show("Error!!!", "Registration error");
                            })));
                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(new Action((() =>
                        {
                            CustomMessageBox.Show("Error!!!", "Same user exists");
                        })));
                    }
                }
            }).ContinueWith(task => { IsRegisterProgress = false; });
        }

        private void ExecuteOnForgotPassword()
        {
            ForgotPassWindow passWindow = new ForgotPassWindow();
            passWindow.Owner = (Window)view;
            passWindow.ShowDialog();
        }

        private bool ValidateOnRegister()
        {
            string message = string.Empty;

            if (string.IsNullOrWhiteSpace(UserLogin))
            {
                message = "Login is empty";
            }
            else if (RPassword.Length < 8 || string.IsNullOrWhiteSpace(RPassword) || RPassword == string.Empty || !Regex.IsMatch(RPassword, @"^[a-zA-Z0-9]{8,}$"))
            {
                message = "Password shoud be 8 symbols lengh,\n use numbers and english symbols";
            }
            else if (RPassword != Rep_RPassword)
            {
                message = "Password not match";
            }
            else if (string.IsNullOrWhiteSpace(Email) || Email == string.Empty || !Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                message = "Wrong Email format";
            }

            if (message != string.Empty)
            {
                Application.Current.Dispatcher.Invoke(new Action((() => { CustomMessageBox.Show("Error", message); })));
                return false;
            }

            return true;
        }

        private bool ValidateOnLogin()
        {
            string message = string.Empty;
            var user = UserServiceClient.GetUser(LoginText, Password);

            if (string.IsNullOrWhiteSpace(LoginText))
            {
                message = "Login is empty";
            }
            else if (Password.Length < 8 || string.IsNullOrWhiteSpace(Password) || Password == string.Empty || !Regex.IsMatch(Password, @"^[a-zA-Z0-9]{8,}$"))
            {
                message = "Password shoud be 8 symbols lenght,\n use numbers and english symbols";
            }
            else if (user == null)
            {
                message = "Wrong login or password";
            }

            if (message != string.Empty)
            {
                Application.Current.Dispatcher.Invoke(new Action((() => { CustomMessageBox.Show("Error", message); })));
                return false;
            }

            return true;
        }

        private void Clear()
        {
            Name = string.Empty;
            Surname = string.Empty;
            UserLogin = string.Empty;
            Email = string.Empty;
            RPassword = string.Empty;
            Rep_RPassword = string.Empty;
        }

        public void UserLeave(User user)
        {
            throw new NotImplementedException();
        }

        public void UserCame(User user)
        {
            throw new NotImplementedException();
        }
    }
}