using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Interfaces
{
    interface IMainView
    {
        void CloseWindow();
        void AnimatedResize(int h, int w);
        void SetOpacity(double opasity);
    }
}
