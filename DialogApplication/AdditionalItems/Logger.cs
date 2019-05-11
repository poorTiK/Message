using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogApplication.AdditionalItems
{
    public static class Logger
    {
        public static void Log(object sender, string message)
        {
            Debug.WriteLine(sender.GetType().Name + " : " + message);
        }
    }
}