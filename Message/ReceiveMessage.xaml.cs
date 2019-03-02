using Message.Model;
using Message.UserServiceReference;
using Message.ViewModel;
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

        public ReceiveMessage(BaseMessage message)
        {
            InitializeComponent();

            MessageText.Text = GlobalBase.Base64Decode(message.Content);
            SendTime.Text = message.DateOfSending.Hour + ":" + message.DateOfSending.Minute;

            DataContext = new MessageControlVM(message);
        }
    }
}