using DialogApplication.AdditionalItems;
using DialogApplication.View.ViewModel;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogApplication.View.Controls.ControlsViewModel
{
    internal class HeaderControlViewModel : BaseViewModel
    {
        private DelegateCommand _closeCommand;
        private DelegateCommand _maximizeCommand;
        private DelegateCommand _minimizeCommand;

        public DelegateCommand CloseCommand =>
            _closeCommand ?? (_closeCommand = new DelegateCommand(ExecuteOnCloseCommand));

        public DelegateCommand MaximizeCommand =>
            _maximizeCommand ?? (_maximizeCommand = new DelegateCommand(ExecuteOnMaximizeCommand));

        public DelegateCommand MinimizeCommand =>
            _minimizeCommand ?? (_minimizeCommand = new DelegateCommand(ExecuteOnMinimizeCommand));

        private bool _maximizeVisible;

        public bool MaximizeVisible
        {
            get
            {
                return _maximizeVisible;
            }
            set
            {
                SetProperty(ref _maximizeVisible, value);
            }
        }

        private bool _minimizeVisible;

        public bool MinimizeVisible
        {
            get
            {
                return _minimizeVisible;
            }
            set
            {
                SetProperty(ref _minimizeVisible, value);
            }
        }

        private string _headerText;

        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                SetProperty(ref _headerText, value);
            }
        }

        public Action OnClose { get; set; }
        public Action OnMaximize { get; set; }
        public Action OnMinimize { get; set; }

        public HeaderControlViewModel(bool minimizeVis = false, bool maximizeVis = false, Action onMinimize = null, Action onMaximize = null, Action onClose = null, string headerText = "")
        {
            MinimizeVisible = minimizeVis;
            MaximizeVisible = maximizeVis;

            HeaderText = headerText;

            OnMinimize = onMinimize;
            OnMaximize = onMaximize;
            OnClose = onClose;
        }

        private void ExecuteOnCloseCommand()
        {
            OnClose?.Invoke();
            Logger.Log(this, "Window Closed");
        }

        private void ExecuteOnMaximizeCommand()
        {
            OnMaximize?.Invoke();
            Logger.Log(this, "Window Maximized");
        }

        private void ExecuteOnMinimizeCommand()
        {
            OnMinimize?.Invoke();
            Logger.Log(this, "Window Minimized");
        }
    }
}