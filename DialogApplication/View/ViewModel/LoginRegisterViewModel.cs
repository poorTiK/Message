using DialogApplication.Interfaces.IView;
using DialogApplication.View.Controls.ControlsViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogApplication.View.ViewModel
{
    internal class LoginRegisterViewModel
    {
        private IViewBase _view;

        public HeaderControlViewModel HeaderControlViewModel { get; set; }

        public LoginRegisterViewModel(IViewBase view)
        {
            _view = view;
            HeaderControlViewModel = new HeaderControlViewModel(true, true, OnMinimize, OnMaximize, OnClose);
        }

        private void OnClose()
        {
            _view.CloseView();
        }

        private void OnMaximize()
        {
            _view.MaximizeView();
        }

        private void OnMinimize()
        {
            _view.MinimizeView();
        }
    }
}