using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using Message.Interfaces;
using Message.UserServiceReference;
using Message.MessageServiceReference;
using Prism.Commands;
using System.Windows;

namespace Message.ViewModel
{
    class ForgotPassWindowVM : Prism.Mvvm.BindableBase, IMessageServiceCallback
    {
        private IView view;
        UserServiceClient userServiceClient;

        public ForgotPassWindowVM(IView view)
        {
            this.view = view;

            userServiceClient = new UserServiceClient();
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

        private DelegateCommand _onSend;
        public DelegateCommand Send =>
            _onSend ?? (_onSend = new DelegateCommand(OnSend));

        private void OnSend()
        {
            if (!string.IsNullOrWhiteSpace(Email))
            {
                var user = userServiceClient.GetAllUsers().First(x => x.Email == Email);
                if (user != null)
                {
                    SendPassWithMail(user);
                }
            }
        }

        private DelegateCommand _onBack;
        public DelegateCommand Back =>
            _onBack ?? (_onBack = new DelegateCommand(OnBack));

        private void OnBack()
        {
            view.CloseWindow();
        }

        private void SendPassWithMail(User user)
        {
            var from = new MailAddress("sh3rgame@gmail.com"); // make custom mail adress
            var to = new MailAddress(user.Email);

            var message = new MailMessage(from, to);
            message.Subject = "Password restore";
            message.Body = "Your pass - " + user.Password;

            var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("sh3rgame@gmail.com", "Ap98Msh77");
            smtp.EnableSsl = true;
            smtp.SendMailAsync(message);
        }

        public void Test()
        {
            throw new System.NotImplementedException();
        }

        public void ReceiveMessage(MessageServiceReference.MessageT message)
        {
            MessageBox.Show("Works");
        }
    }
}
