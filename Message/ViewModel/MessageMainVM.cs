using Message.Interfaces;
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
    }
}
