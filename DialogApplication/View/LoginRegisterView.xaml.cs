using DialogApplication.Interfaces.IView;
using DialogApplication.View.ViewModel;
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

namespace DialogApplication.View
{
    /// <summary>
    /// Interaction logic for LoginRegisterView.xaml
    /// </summary>
    public partial class LoginRegisterView : Window, IViewBase
    {
        public LoginRegisterView()
        {
            InitializeComponent();

            DataContext = new LoginRegisterViewModel(this);
        }

        public void CloseView()
        {
            Close();
        }

        public void MaximizeView()
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        public void MinimizeView()
        {
            WindowState = WindowState.Minimized;
        }
    }
}