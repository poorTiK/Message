using Message.Interfaces;
using Message.Model;
using Message.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Message
{
    /// <summary>
    /// Interaction logic for UserProfileWindow.xaml
    /// </summary>
    public partial class UserProfileWindow : Window, IView
    {
        public UserProfileWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = GlobalBase.Language;

            InitializeComponent();

            DataContext = new UserProfileWindowVM(this);
        }

        public void AnimatedResize(int h, int w)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}