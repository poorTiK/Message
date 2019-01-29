using ServerWCF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Services
{
    public class MessageTService : IMessageTService
    {
        public int Test()
        {
            return 2;
        }
    }
}
