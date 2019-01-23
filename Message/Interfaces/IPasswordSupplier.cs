using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Interfaces
{
    public interface IPasswordSupplier
    {
        string GetPasswordForLogin();
        string GetPasswordForRegistration();
        string GetRepPasswordForRegistration();
    }
}
