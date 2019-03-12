using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Message
{
    /// <summary>
    /// Interaction logic for ChatSettingWindow.xaml
    /// </summary>
    public partial class ChatSettingWindow : Window
    {
        public ChatSettingWindow()
        {
            InitializeComponent();
            
            CultureInfo currLang = App.Language;

            LangComboBox.Items.Clear();

            foreach (var language in App.Languages)
            {
                var item = new ComboBoxItem() {Content = language.DisplayName};
                item.Tag = language;
                item.Selected += ItemOnSelected;
                item.IsSelected = language.Equals(currLang);
                LangComboBox.Items.Add(item);
            }
        }

        private void ItemOnSelected(object sender, RoutedEventArgs e)
        {
            var item = sender as ComboBoxItem;
            if (item != null)
            {
                var lang = item.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
