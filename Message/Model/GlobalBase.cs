using System;
using Message.UserServiceReference;
using Notifications.Wpf;
using System.Globalization;

namespace Message.Model
{
    public static class GlobalBase
    {
        public static CultureInfo Language { get; set; }

        public static User CurrentUser { get; set; }

        public static Action UpdateUI;

        static GlobalBase()
        {
            Language = CultureInfo.CurrentUICulture;
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

        /// <summary>
        /// Example - CultureInfo.GetCultureInfo("en-US");
        /// </summary>
        /// <param name="lang"></param>
        public static void SetLanguage(CultureInfo lang)
        {
            Language = lang;
        }

        public static bool IsRussianLanguage()
        {
            if (Language.Name == "ru-UA" || Language.Name == "ru-RU")
                return true;
            return false;
        }
    }
}