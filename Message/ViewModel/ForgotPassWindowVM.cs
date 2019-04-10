using Message.AdditionalItems;
using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Prism.Commands;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
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

                var message = new MailMessage(from, to);
                message.Subject = "Password restore";
                message.Body = "Your pass - " + user.Password;

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
    }
}