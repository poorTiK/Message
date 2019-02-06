using Message.Interfaces;
using Message.ViewModel;
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
    /// Interaction logic for Contacts.xaml
    /// </summary>
    public partial class Contacts : Window, IView
    {
        public Contacts()
        {
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
    }
}
