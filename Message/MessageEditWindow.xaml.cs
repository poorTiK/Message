using Message.AdditionalItems;
using Message.Interfaces;
using Message.UserServiceReference;
using Message.ViewModel;
using System;
using System.Windows;

namespace Message
{
    /// <summary>
    /// Interaction logic for MessageEditWindow.xaml
    /// </summary>
    public partial class MessageEditWindow : Window, IView
    {
        public MessageEditWindow(BaseMessage message)
        {
            InitializeComponent();

            DwmDropShadow.DropShadowToWindow(this);

            DataContext = new EditMessageVM(message, this);
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
    }
}