using Message.Interfaces;
using Message.Model;
using Message.ViewModel;
using System;
using System.Windows;

namespace Message
{
    /// <summary>
    /// Interaction logic for Contacts.xaml
    /// </summary>
    public partial class Contacts : Window, IView
    {
        public Contacts()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = GlobalBase.Language;

            InitializeComponent();
            DataContext = new ContatsWindowVM(this);

            Loaded += Contacts_Loaded;
        }

        private void Contacts_Loaded(object sender, RoutedEventArgs e)
        {
            SearchBox.Focus();
        }

        public void AnimatedResize(int h, int w)
        {
            throw new NotImplementedException();
        }

        public void CloseWindow()
        {
            Close();
        }

        public void SetOpacity(double opacity)
        {
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
    }
}