using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogApplication.Interfaces.IView
{
    internal interface IViewBase
    {
        void MinimizeView();

        void MaximizeView();

        void CloseView();
    }
}