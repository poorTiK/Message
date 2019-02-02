using Message.Interfaces;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.ViewModel
{
    class ContatsWindowVM : Prism.Mvvm.BindableBase
    {
        IView view;

        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set { SetProperty(ref _searchString, value); }
        }

        public ContatsWindowVM(IView iview)
        {
            view = iview;
        }

        private DelegateCommand _onAddContact;
        public DelegateCommand AddContactCommand =>
            _onAddContact ?? (_onAddContact = new DelegateCommand(ExecuteOnAddContactCommand));

        private DelegateCommand _onClose;
        public DelegateCommand CloseCommand =>
            _onClose ?? (_onClose = new DelegateCommand(ExecuteOnCloseCommand));

        private void ExecuteOnCloseCommand()
        {
            view.CloseWindow();
        }

        private void ExecuteOnAddContactCommand()
        {
            
        }
    }
}
