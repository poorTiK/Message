using Message.AdditionalItems;
using Message.Interfaces;
using Message.UserServiceReference;
using Message.ViewModel;
using System.Windows;

namespace Message
{
    /// <summary>
    /// Interaction logic for SelectUsersWindow.xaml
    /// </summary>
    public partial class SelectUsersWindow : Window, IView
    {
        public SelectUsersWindow(ChatGroup group)
        {
            InitializeComponent();

            DwmDropShadow.DropShadowToWindow(this);

            DataContext = new SelectUsersWindowVM(this, group);
        }

        public void AnimatedResize(int h, int w)
        {
            //throw new NotImplementedException();
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
            //throw new NotImplementedException();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}