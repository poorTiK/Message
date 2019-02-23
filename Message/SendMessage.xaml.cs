using System;
using System.Windows.Controls;

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
            SendTime.Text = date.Hour + ":" + date.Minute;
        }
    }
}