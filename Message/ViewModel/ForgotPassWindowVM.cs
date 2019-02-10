using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Message.Interfaces;
using Prism.Commands;

namespace Message.ViewModel
{
    class ForgotPassWindowVM : Prism.Mvvm.BindableBase
    {
        private IView view;

        public ForgotPassWindowVM(IView view)
        {
            this.view = view;
            IsLogin = true;
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

        private string _isMail;
        public string IsMail
        {
            get { return _isMail; }
            set { SetProperty(ref _isMail, value); }
        }

        private DelegateCommand _onSend;
        public DelegateCommand Send =>
            _onSend ?? (_onSend = new DelegateCommand(OnSend));

        private void OnSend()
        {
            if (!string.IsNullOrWhiteSpace(Email)) //find user with this mail
            {
                SendPassWithMail(Email);
            }
        }

        private DelegateCommand _onBack;
        public DelegateCommand Back =>
            _onBack ?? (_onBack = new DelegateCommand(OnBack));

        private void OnBack()
        {
            view.CloseWindow();
        }

        private void SendPassWithMail(string email)
        {
            MailAddress from = new MailAddress("sh3rgame@gmail.com"); // make custom mail adress
            MailAddress to = new MailAddress(email);

            MailMessage message = new MailMessage(from, to);
            message.Subject = "Password restore";
            message.Body = "Your pass - ...";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("sh3rgame@gmail.com", "Ap98Msh77");
            smtp.EnableSsl = true;
            smtp.SendMailAsync(message);
        }
    }
}
