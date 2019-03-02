using Message.Interfaces;
using Message.Model;
using Message.UserServiceReference;
using Message.ViewModel;
using System;
using System.Windows;

namespace Message
{
    /// <summary>
    /// Interaction logic for ContactProfileWindow.xaml
    /// </summary>
    public partial class ContactProfileWindow : Window, IView
    {
        public ContactProfileWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = GlobalBase.Language;

            InitializeComponent();
        }

        public ContactProfileWindow(User profile)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = GlobalBase.Language;

            InitializeComponent();

            DataContext = new ContactProfileWindowVM(this, profile);
        }

        public void AnimatedResize(int h, int w)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}