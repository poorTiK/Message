using Message.Interfaces;
using Message.UserServiceReference;
using Message.ViewModel;
using System.Windows;

namespace Message
{
    /// <summary>
    /// Interaction logic for EditGroupWindow.xaml
    /// </summary>
    public partial class EditGroupWindow : Window, IView
    {
        public EditGroupWindow(ChatGroupUiInfo group)
        {
            InitializeComponent();

            DataContext = new EditGroupWindowVM(this, group);
        }

        public void AnimatedResize(int h, int w)
        {
            //
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
            //
        }

        private void ButtonClose_OnClickose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}