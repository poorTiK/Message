using Message.Interfaces;
using Message.Model;
using Message.ViewModel;
using System;
using System.Windows;

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

        public void SetOpacity(double opacity)
        {
            throw new NotImplementedException();
        }
    }
}