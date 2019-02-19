using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Message
{
    /// <summary>
    /// Interaction logic for SendMessage.xaml
    /// </summary>
    public partial class SendMessage : UserControl
    {
        public SendMessage()
        {
            InitializeComponent();
        }

        public SendMessage(string text, DateTime date)
        {
            InitializeComponent();

            MessageText.Text = text;
            SendTime.Text = date.ToString();
        }
    }
}
