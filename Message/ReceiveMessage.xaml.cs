﻿using System;
using System.Windows;
using System.Windows.Controls;
using Message.FileService;
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

            FileService.ChatFile chatFile = GlobalBase.FileServiceClient.getChatFileById(message.ChatFileId);
            //MessageText.Text = message.ChatFileId == 0 ? GlobalBase.Base64Decode(chatFile?.Source) : "File";
            MessageText.Text = GlobalBase.Base64Decode(message.Text);
            SendTime.Text = message.DateOfSending.Hour + ":" + message.DateOfSending.Minute;
            ButtonDwnld.Visibility = message.ChatFileId != 0 ? Visibility.Visible : Visibility.Collapsed;

            if (message is GroupMessage)
            {
                SenderName.Visibility = Visibility.Visible;
                SenderName.Text = message.Sender.FirstName;
            }
            else
            {
                SenderName.Visibility = Visibility.Collapsed;
            }
            

            DataContext = new MessageControlVM(message);
        }
    }
}