using System;
using System.Windows.Controls;
using Message.Model;
using Message.UserServiceReference;
using Message.ViewModel;

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

        public SendMessage(BaseMessage message)
        {
            InitializeComponent();

            MessageText.Text = GlobalBase.Base64Decode(message.Content);
            SendTime.Text = message.DateOfSending.Hour + ":" + message.DateOfSending.Minute;

            DataContext = new MessageControlVM(message);
        }
    }
}