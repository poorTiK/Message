using System;
using Message.UserServiceReference;
using Notifications.Wpf;
using System.Globalization;
using Message.PhotoServiceReference;
using System.IO;
using System.Linq;
using Message.FileService;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Drawing;

namespace Message.Model
{
    public static class GlobalBase
    {
        public static User CurrentUser { get; set; }

        public static Action UpdateUI;

        public static PhotoServiceClient PhotoServiceClient { get; set; }

        public static FileServiceClient FileServiceClient { get; set; } 

        static GlobalBase()
        {
            PhotoServiceClient = new PhotoServiceClient();
            FileServiceClient = new FileServiceClient();
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

        public static byte[] FileToByte(string path)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(path,
                FileMode.Open,
                FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(path).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        public static string GetShortName(string fullPath)
        {
            string[] pathArr = fullPath.Split('\\');
            string fileName = pathArr.Last().ToString();

            return fileName;
        } 

        public static void loadPictures(UserServiceClient userServiceClient, List<UiInfo> uiInfos)
        {
            foreach (var item in uiInfos)
            {
                if (item is UserUiInfo)
                {
                    UserUiInfo userUiInfo = item as UserUiInfo;
                    User user = userServiceClient.GetUserById(userUiInfo.UserId);
                    ChatFile chatFile = GlobalBase.FileServiceClient.getChatFileById(user.Id);

                    if (chatFile?.Source?.Length > 0)
                    {
                        MemoryStream memstr = new MemoryStream(chatFile.Source);
                        Dispatcher.CurrentDispatcher.Invoke(() => { item.UiImage = Image.FromStream(memstr); });
                    }
                    else
                    {
                        Dispatcher.CurrentDispatcher.Invoke(() => { item.UiImage = Image.FromFile(@"../../Resources/DefaultPicture.jpg"); });
                    }
                }
            }
        }

        public static void loadPictureForUser(User user)
        {

        }
    }
}