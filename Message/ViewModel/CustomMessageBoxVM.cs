using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.ViewModel
{
    public enum CustomMessageBoxResult {
        Yes,
        No,
        Ok,
        Cancel
    }
    public enum CustomMessageBoxType
    {
        YesNo,
        Ok,
        OkCancel
    }
    class CustomMessageBoxVM : Prism.Mvvm.BindableBase
    {

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set { SetProperty(ref _caption, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private bool _yesNoVisibility;
        public bool YesNoVisibility
        {
            get { return _yesNoVisibility; }
            set { SetProperty(ref _yesNoVisibility, value); }
        }

        private bool _okVisibility;
        public bool OkVisibility
        {
            get { return _okVisibility; }
            set { SetProperty(ref _okVisibility, value); }
        }

        private bool _cancelVisibility;
        public bool CancelVisibility
        {
            get { return _cancelVisibility; }
            set { SetProperty(ref _cancelVisibility, value); }
        }

        public CustomMessageBoxType Type { get; set; } = CustomMessageBoxType.Ok;

        public CustomMessageBoxVM(string caption, string message, CustomMessageBoxType customMessageBoxType = CustomMessageBoxType.Ok)
        {
            Caption = caption;
            Message = message;
            Type = customMessageBoxType;

            HideAll();

            switch (Type)
            {
                case CustomMessageBoxType.YesNo:
                    YesNoVisibility = true;
                    break;
                case CustomMessageBoxType.Ok:
                    OkVisibility = true;
                    break;
                case CustomMessageBoxType.OkCancel:
                    OkVisibility = true;
                    CancelVisibility = true;
                    break;
                default:
                    break;
            }
        }

        private void HideAll()
        {
            YesNoVisibility = false;
            OkVisibility = false;
            CancelVisibility = false;
        }
    }
}
