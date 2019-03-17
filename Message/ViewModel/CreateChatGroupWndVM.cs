using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Message.Interfaces;

namespace Message.ViewModel
{
    class CreateChatGroupWndVM : BaseViewModel
    {
        private IView _view;

        public CreateChatGroupWndVM(IView view)
        {
            _view = view;
        }


    }
}
