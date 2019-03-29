using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Message.Model
{
    public static class Translations
    {
        public static ResourceDictionary GetTranslation()
        {
            return Application.Current.Resources.MergedDictionaries[4];
        }
    }
}
