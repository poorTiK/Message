using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Message.AdditionalItems;

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

            DwmDropShadow.DropShadowToWindow(this);

            CultureInfo currLang = App.Language;

            LangComboBox.Items.Clear();

            foreach (var language in App.Languages)
            {
                var item = new ComboBoxItem() { Content = language.DisplayName };
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

        private void DarkTheme_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dict = new ResourceDictionary();
            dict.Source = new Uri("Resources/theme.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.RemoveAt(5);
            Application.Current.Resources.MergedDictionaries.Add(dict);

            SaveTheme();
        }

        private void LightTheme_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dict = new ResourceDictionary();
            dict.Source = new Uri("Resources/theme_Light.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.RemoveAt(5);
            Application.Current.Resources.MergedDictionaries.Add(dict);

            SaveTheme();
        }

        private void GreenTheme_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dict = new ResourceDictionary();
            dict.Source = new Uri("Resources/theme_Green.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.RemoveAt(5);
            Application.Current.Resources.MergedDictionaries.Add(dict);

            SaveTheme();
        }

        public void SaveTheme()
        {
            Properties.Settings.Default.DefaultTheme = Application.Current.Resources.MergedDictionaries.ElementAt(5).Source.OriginalString;
            Properties.Settings.Default.Save();
        }
    }
}