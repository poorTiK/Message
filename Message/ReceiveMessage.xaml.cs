using System;
using System.Windows.Controls;

namespace Message
{
    /// <summary>
    /// Interaction logic for ReceiveMessage.xaml
    /// </summary>
    public partial class ReceiveMessage : UserControl
    {
        public ReceiveMessage()
        {
            InitializeComponent();
        }

        public ReceiveMessage(string text, DateTime date)
        {
            InitializeComponent();

            MessageText.Text = text;
            SendTime.Text = date.Hour + ":" + date.Minute;
        }
    }
}