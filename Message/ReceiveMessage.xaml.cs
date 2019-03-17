using System;
using System.Windows;
using System.Windows.Controls;
using Message.Model;
using Message.UserServiceReference;
using Message.ViewModel;

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

            MessageText.Text = message.Type == "TEXT" ? GlobalBase.Base64Decode(message.Content) : "File";
            SendTime.Text = message.DateOfSending.Hour + ":" + message.DateOfSending.Minute;
            ButtonDwnld.Visibility = message.Type == "DATA" ? Visibility.Visible : Visibility.Collapsed;

            DataContext = new MessageControlVM(message);
        }
    }
}