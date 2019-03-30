using Message.Interfaces;
using Message.UserServiceReference;
using Message.ViewModel;
using System.Windows;

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