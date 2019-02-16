using Message.ViewModel;
using System;
using System.Collections.Generic;
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
using Message.Interfaces;

namespace Message
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, IView
    {
        public SettingsWindow()
        {
            InitializeComponent();

            DataContext = new SettingsWindowVM(this);
        }

        public void AnimatedResize(int h, int w)
        {
            throw new NotImplementedException();
        }

        public void CloseWindow()
        {
            Close();
        }

        public void SetOpacity(double opacity)
        {
            throw new NotImplementedException();
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
