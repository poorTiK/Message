﻿using System;
using System.Windows;
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

            //MessageText.Text = message.ChatFileId == 0 ? GlobalBase.Base64Decode(message.Text) : "File";
            MessageText.Text = GlobalBase.Base64Decode(message.Text);
            SendTime.Text = message.DateOfSending.Hour + ":" + message.DateOfSending.Minute;
            ButtonDwnld.Visibility = message.ChatFileId != 0 ? Visibility.Visible : Visibility.Collapsed;

            DataContext = new MessageControlVM(message);
        }
    }
}