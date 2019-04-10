using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Interfaces
{
    internal interface IRestorePasswordSupplier
    {
        string GetCurrentPassword();

        string GetNewPassword();
    }
}