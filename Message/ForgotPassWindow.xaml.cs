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
using Message.ViewModel;

namespace Message
{
    /// <summary>
    /// Interaction logic for ForgotPassWindow.xaml
    /// </summary>
    public partial class ForgotPassWindow : Window, IView
    {
        public ForgotPassWindow()
        {
            InitializeComponent();

            DataContext = new ForgotPassWindowVM(this);
        }

        public void AnimatedResize(int h, int w)
        {
            throw new NotImplementedException();
        }

        public void CloseWindow()
        {
            Close();
        }
    }
}
