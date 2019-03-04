using Message.Interfaces;
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
using Message.UserServiceReference;
using Message.ViewModel;

namespace Message
{
    /// <summary>
    /// Interaction logic for ForwardMessageWindow.xaml
    /// </summary>
    public partial class ForwardMessageWindow : Window, IView
    {
        public ForwardMessageWindow(BaseMessage message)
        {
            InitializeComponent();

            DataContext = new ForwardMessageWindowVM(message, this);
        }

        public void AnimatedResize(int h, int w)
        {
        }

        public void CloseWindow()
        {
            Close();
        }

        public void Hide(bool isVisible)
        {
            if (isVisible)
            {
                Visibility = Visibility.Visible;
            }
            else
            {
                Visibility = Visibility.Hidden;
            }
        }

        public void SetOpacity(double opasity)
        {
        }
    }
}
