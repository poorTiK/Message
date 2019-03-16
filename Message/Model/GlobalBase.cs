﻿using System;
using Message.UserServiceReference;
using Notifications.Wpf;
using System.Globalization;
using Message.PhotoServiceReference;

namespace Message.Model
{
    public static class GlobalBase
    {
        public static User CurrentUser { get; set; }

        public static Action UpdateUI;

        public static PhotoServiceClient PhotoServiceClient { get; set; }

        static GlobalBase()
        {
            PhotoServiceClient = new PhotoServiceClient();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(byte[] base64EncodedData)
        {
            return System.Text.Encoding.UTF8.GetString(base64EncodedData);
        }

        public static void ShowNotify(string title, string content)
        {
            var notificationManager = new NotificationManager();

            notificationManager.Show(new NotificationContent
            {
                Title = title,
                Message = content,
                Type = NotificationType.Information
            });
        }
    }
}