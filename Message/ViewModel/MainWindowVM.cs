using Message.AdditionalItems;
using Message.Encryption;
using Message.Interfaces;
using Message.Model;
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
    internal class MainWindowVM : BaseViewModel
    {
        #region PrivateReadOnly

        private readonly ISerializeUser _serializeUser;

        #endregion PrivateReadOnly

        private IView view;
        private IPasswordSupplier passwordSupplier;

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
            set { SetProperty(ref _isLoginProgress, value); IsLoginNotPregress = !_isLoginProgress; }
        }

        private bool _isLoginNotProgress;

        public bool IsLoginNotPregress
        {
            get { return _isLoginNotProgress; }
            set { SetProperty(ref _isLoginNotProgress, value); }
        }

        private bool _isRegisterProgress;

        public bool IsRegisterProgress
        {
            get { return _isRegisterProgress; }
            set { SetProperty(ref _isRegisterProgress, value); IsRegisterNotProgress = !_isRegisterProgress; }
        }

        private bool _isRegisterNotProgress;

        public bool IsRegisterNotProgress
        {
            get { return _isRegisterNotProgress; }
            set { SetProperty(ref _isRegisterNotProgress, value); }
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
            set
            {
                SetProperty(ref _password, value);
            }
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

        public MainWindowVM(IView iView, IPasswordSupplier ipasswordSupplier) : base()
        {
            view = iView;
            passwordSupplier = ipasswordSupplier;
            _serializeUser = new SerializeUserToRegistry();

            IsLoginProgress = true;

            var userLogin = TryGetUser();

            if (!string.IsNullOrEmpty(userLogin))
            {
                InitLogin(userLogin);
            }

            //IsLoginProgress = false;
            IsRegisterProgress = false;

            IsSignUpVisible = true;
            IsRegisterVisible = false;
        }

        private void InitLogin(string userLogin)
        {
            var user = new User();
            Task.Run(() =>
            {
                user = UserServiceClient.GetUserByLogin(userLogin);
            }).ContinueWith(task =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (user?.Status == "online")
                    {
                        CustomMessageBox.Show(
                        Translations.GetTranslation()["Error"].ToString(),
                        Translations.GetTranslation()["UserAlreadyOnline"].ToString(),
                        MessageBoxType.Error);

                        IsLoginProgress = false;
                        return;
                    }
                    else if (user == null)
                    {
                        IsLoginProgress = false;
                        return;
                    }

                    var wnd = new MessageMainWnd(user);
                    wnd.Show();
                    view.CloseWindow();
                    IsLoginProgress = false;
                });
            });
        }

        private string TryGetUser()
        {
            try
            {
                return _serializeUser.GetCurrentUser();
            }
            catch (Exception)
            {
                return string.Empty;
            }
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

        private async void ExecuteOnLogin()
        {
            IsLoginProgress = true;
            await Task.Factory.StartNew(() =>
            {
                if (ValidateOnLogin())
                {
                    var user = UserServiceClient.GetUser(LoginText, AESEncryptor.encryptPassword(Password));
                    if (user != null)
                    {
                        if (user.Status == "online")
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                CustomMessageBox.Show(
                                Translations.GetTranslation()["Error"].ToString(),
                                Translations.GetTranslation()["UserAlreadyOnline"].ToString(),
                                MessageBoxType.Error);
                            });
                            return;
                        }
                        _serializeUser.SerializeUser(user);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var wnd = new MessageMainWnd(user);
                            wnd.Show();
                            view.CloseWindow();
                            IsLoginProgress = false;
                        });
                    }
                }
            }).ContinueWith(task => { IsLoginProgress = false; });
        }

        private void ExecuteOnRegister()
        {
            IsRegisterProgress = true;
            Task.Run(() =>
            {
                var message = string.Empty;
                if (ValidateOnRegister())
                {
                    var user = new User()
                    {
                        Login = UserLogin,
                        Password = AESEncryptor.encryptPassword(RPassword),
                        FirstName = Name,
                        LastName = Surname,
                        Email = Email,
                        Status = DateTime.Now.ToString()
                    };

                    if (UserServiceClient.GetUserByLogin(UserLogin) == null)
                    {
                        if (UserServiceClient.AddOrUpdateUser(user) == string.Empty)
                        {
                            Application.Current.Dispatcher.Invoke(new Action((() =>
                            {
                                CustomMessageBox.Show(Translations.GetTranslation()["RegisterDone"].ToString());
                                Clear();
                                ExecuteOnBackCommand();
                                return;
                            })));
                        }
                        else
                        {
                            message = Translations.GetTranslation()["RegError"].ToString();
                        }
                    }
                    else
                    {
                        message = Translations.GetTranslation()["SameUserExits"].ToString();
                    }

                    if (message != string.Empty)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            CustomMessageBox.Show(Translations.GetTranslation()["Error"].ToString(), message);
                        }));
                    }
                }
            }).ContinueWith(task => { IsRegisterProgress = false; });
        }

        private void ExecuteOnForgotPassword()
        {
            var passWindow = new ForgotPassWindow();
            passWindow.Owner = (Window)view;
            passWindow.ShowDialog();
        }

        private bool ValidateOnRegister()
        {
            var message = string.Empty;
            if (string.IsNullOrWhiteSpace(Name))
            {
                message = Translations.GetTranslation()["FirstNameValid"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(Surname))
            {
                message = Translations.GetTranslation()["LastNameValid"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(UserLogin))
            {
                message = Translations.GetTranslation()["EmptyLogin"].ToString();
            }
            else if (RPassword.Length < 8 || string.IsNullOrWhiteSpace(RPassword) || RPassword == string.Empty || !Regex.IsMatch(RPassword, @"^[a-zA-Z0-9]{8,}$"))
            {
                message = Translations.GetTranslation()["PassValidation"].ToString();
            }
            else if (RPassword != Rep_RPassword)
            {
                message = Translations.GetTranslation()["PassMatchValidation"].ToString();
            }
            else if (string.IsNullOrWhiteSpace(Email) || Email == string.Empty || !Regex.IsMatch(Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                message = Translations.GetTranslation()["EmailValidation"].ToString();
            }

            if (message != string.Empty)
            {
                Application.Current.Dispatcher.Invoke(new Action((() => { CustomMessageBox.Show(Translations.GetTranslation()["Error"].ToString(), message); })));
                return false;
            }

            return true;
        }

        private bool ValidateOnLogin()
        {
            var message = string.Empty;

            if (string.IsNullOrWhiteSpace(LoginText))
            {
                message = Translations.GetTranslation()["EmptyLogin"].ToString();
            }
            else if (Password.Length < 8 || string.IsNullOrWhiteSpace(Password) || Password == string.Empty || !Regex.IsMatch(Password, @"^[a-zA-Z0-9]{8,}$"))
            {
                message = Translations.GetTranslation()["PassValidation"].ToString();
            }
            else if (UserServiceClient.GetUser(LoginText, AESEncryptor.encryptPassword(Password)) == null)
            {
                message = Translations.GetTranslation()["LogPassValid"].ToString();
            }

            if (message != string.Empty)
            {
                Application.Current.Dispatcher.Invoke(new Action((() => { CustomMessageBox.Show(Translations.GetTranslation()["Error"].ToString(), message); })));
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
    }
}