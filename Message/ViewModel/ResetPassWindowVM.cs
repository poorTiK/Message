using Message.AdditionalItems;
using Message.Encryption;
using Message.Interfaces;
using Message.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Message.ViewModel
{
    internal class ResetPassWindowVM : BaseViewModel
    {
        private IView _view;
        private IRestorePasswordSupplier _restorePasswordSupplier;

        private string _oldPass;
        private string _newPass;

        private bool _isResetingNotProgress;

        public bool IsResetingNotProgress
        {
            get { return _isResetingNotProgress; }
            set { SetProperty(ref _isResetingNotProgress, value); }
        }

        private bool _isResetingProgress;

        public bool IsResetingProgress
        {
            get { return _isResetingProgress; }
            set { SetProperty(ref _isResetingProgress, value); }
        }

        public ResetPassWindowVM(IView view, IRestorePasswordSupplier restorePasswordSupplier)
        {
            _view = view;
            _restorePasswordSupplier = restorePasswordSupplier;

            IsResetingNotProgress = true;
            IsResetingProgress = false;
        }

        private DelegateCommand _onApply;

        public DelegateCommand Apply =>
            _onApply ?? (_onApply = new DelegateCommand(OnApply));

        private DelegateCommand _back;

        public DelegateCommand Back =>
            _back ?? (_back = new DelegateCommand(OnBack));

        private void OnApply()
        {
            _oldPass = _restorePasswordSupplier.GetCurrentPassword();
            _newPass = _restorePasswordSupplier.GetNewPassword();

            if (Validate())
            {
                IsResetingNotProgress = false;
                IsResetingProgress = true;

                var result = string.Empty;

                Task.Run(() =>
                {
                    try
                    {
                        GlobalBase.CurrentUser.Password = AESEncryptor.encryptPassword(_newPass);

                        UserServiceClient.AddOrUpdateUser(GlobalBase.CurrentUser);
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }).ContinueWith(task =>
                {
                    if (result == string.Empty)
                    {
                        Application.Current.Dispatcher.Invoke(new Action((() =>
                        {
                            CustomMessageBox.Show(Translations.GetTranslation()["ChangesSaved"].ToString());
                            OnBack();
                            return;
                        })));
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(new Action((() =>
                        {
                            CustomMessageBox.Show(Translations.GetTranslation()["Error"].ToString(), result);
                        })));
                    }
                });
            }
        }

        private void OnBack()
        {
            _view.CloseWindow();
        }

        private bool Validate()
        {
            var message = string.Empty;
            if (string.IsNullOrWhiteSpace(_oldPass))
            {
                message = Translations.GetTranslation()["OldPassValid"].ToString();
            }
            else if (AESEncryptor.decryptPassword(GlobalBase.CurrentUser.Password) != _oldPass)
            {
                message = Translations.GetTranslation()["ResPassCurPassNotMatch"].ToString();
            }
            else if (_newPass.Length < 8 || string.IsNullOrWhiteSpace(_newPass) || _newPass == string.Empty || !Regex.IsMatch(_newPass, @"^[a-zA-Z0-9]{8,}$"))
            {
                message = Translations.GetTranslation()["PassValidation"].ToString();
            }

            if (message != string.Empty)
            {
                Application.Current.Dispatcher.Invoke(new Action((() => { CustomMessageBox.Show(Translations.GetTranslation()["Error"].ToString(), message); })));
                return false;
            }

            return true;
        }
    }
}