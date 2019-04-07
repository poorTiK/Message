using Message.AdditionalItems;
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

namespace Message
{
    /// <summary>
    /// Interaction logic for SelectUsersWindow.xaml
    /// </summary>
    public partial class SelectUsersWindow : Window, IView
    {
        public SelectUsersWindow()
        {
            InitializeComponent();

            DwmDropShadow.DropShadowToWindow(this);

            //DataContext = new
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