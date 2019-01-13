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

namespace Message.AdditionalItems
{
    /// <summary>
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox()
        {
            InitializeComponent();
        }

        public CustomMessageBox(string caption, string message, CustomMessageBoxType type = CustomMessageBoxType.Ok)
        {
            InitializeComponent();

            CloseButton.Click += (s, e) => Close();

            DataContext = new CustomMessageBoxVM(caption, message, type);
        }
    }
}
