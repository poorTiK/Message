using Message.AdditionalItems;
using Message.Encryption;
using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Message.ViewModel
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class ForgotPassWindowVM : BaseViewModel
    {
        private IView view;

        public ForgotPassWindowVM(IView view) : base()
        {
            this.view = view;

            IsLogin = true;
            IsMail = false;
        }

        private string _login;

        public string Login
        {
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        private bool _isLogin;

        public bool IsLogin
        {
            get { return _isLogin; }
            set { SetProperty(ref _isLogin, value); }
        }

        private bool _isMail;

        public bool IsMail
        {
            get { return _isMail; }
            set { SetProperty(ref _isMail, value); }
        }

        private bool _isSending;

        public bool IsSending
        {
            get { return _isSending; }
            set { SetProperty(ref _isSending, value); }
        }

        private DelegateCommand _onSend;

        public DelegateCommand Send =>
            _onSend ?? (_onSend = new DelegateCommand(OnSend));

        private void OnSend()
        {
            try
            {
                IsSending = true;

                var ts = new CancellationTokenSource();
                CancellationToken ct = ts.Token;
                Task.Factory.StartNew(() =>
                {
                    if (!string.IsNullOrWhiteSpace(Email))
                    {
                        User user = UserServiceClient.GetAllUsers().First(x => x.Email == Email);

                        if (user != null)
                        {
                            SendPassWithMail(user);
                            return;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(Login))
                    {
                        User user = UserServiceClient.GetUserByLogin(Login);

                        if (user != null)
                        {
                            SendPassWithMail(user);
                            return;
                        }
                    }

                    Application.Current.Dispatcher.Invoke(new Action((() =>
                    {
                        CustomMessageBox.Show(Translations.GetTranslation()["Error"].ToString(), Application.Current.Resources.MergedDictionaries[4]["CantFindUser"].ToString());
                        ts.Cancel();
                    })));
                }, ct).ContinueWith((task =>
                {
                    if (!ct.IsCancellationRequested)
                    {
                        Application.Current.Dispatcher.Invoke(new Action((() =>
                        {
                            CustomMessageBox.Show(Translations.GetTranslation()["RestorePass"].ToString(), Application.Current.Resources.MergedDictionaries[4]["EmailSend"].ToString());
                        })));
                    }
                    IsSending = false;
                }));
            }
            finally
            {

            }
        }

        private DelegateCommand _onBack;

        public DelegateCommand Back =>
            _onBack ?? (_onBack = new DelegateCommand(OnBack));

        private void OnBack()
        {
            try
            {
                view.CloseWindow();
            }
            finally
            {

            }
        }

        private void SendPassWithMail(User user)
        {
            try
            {
                var from = new MailAddress("messagePassRestoration@gmail.com"); // make custom mail adress
                var to = new MailAddress(user.Email);

            var newPas = AESEncryptor.encryptPassword(RandomNumberGenerator.RandomPassword());
            user.Password = newPas;

            UserServiceClient.AddOrUpdateUser(user);

            var message = new MailMessage(from, to);
            message.Subject = "Password restore";
            message.Body = "Your pass - " + AESEncryptor.decryptPassword(newPas);

                var smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("messagePassRestoration@gmail.com", "messageApp1");
                smtp.EnableSsl = true;
                smtp.SendMailAsync(message);

                Application.Current.Dispatcher.Invoke(new Action((() =>
                {
                    try
                    {
                        CustomMessageBox.Show(Translations.GetTranslation()["RestorePass"].ToString(), Application.Current.Resources.MergedDictionaries[4]["EmailSend"].ToString());
                        IsSending = false;
                    }
                    finally
                    {

                    }
                })));
            }
            finally
            {

            }
        }

        public static class RandomNumberGenerator
        {
            private static int RandomNumber(int min, int max)
            {
                var random = new Random();
                return random.Next(min, max);
            }

            private static string RandomString(int size, bool lowerCase)
            {
                var builder = new StringBuilder();
                var random = new Random();
                char ch;
                for (var i = 0; i < size; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                if (lowerCase)
                    return builder.ToString().ToLower();
                return builder.ToString();
            }

            public static string RandomPassword()
            {
                var builder = new StringBuilder();
                builder.Append(RandomString(4, true));
                builder.Append(RandomNumber(1000, 9999));
                builder.Append(RandomString(2, false));
                return builder.ToString();
            }
        }
    }
}