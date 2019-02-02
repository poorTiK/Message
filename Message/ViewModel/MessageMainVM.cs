using Message.Interfaces;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.ViewModel
{
    class MessageMainVM : Prism.Mvvm.BindableBase
    {
        IView _view;

        

        public MessageMainVM(IView View)
        {
            _view = View;
        }

        private DelegateCommand _onContactsCommand;
        public DelegateCommand ContactsCommand =>
            _onContactsCommand ?? (_onContactsCommand = new DelegateCommand(ExecuteOnContacts));

        private DelegateCommand _onSettingsCommand;
        public DelegateCommand SettingsCommand =>
            _onSettingsCommand ?? (_onSettingsCommand = new DelegateCommand(ExecuteOnSettingsCommand));

        private void ExecuteOnSettingsCommand()
        {
            //throw new NotImplementedException();
        }

        private void ExecuteOnContacts()
        {
            var wnd = new Contacts();
            wnd.ShowDialog();
        }
    }
}
