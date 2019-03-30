using Message.FileService;
using Message.UserServiceReference;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Threading;

namespace Message.Model
{
    public static class GlobalBase
    {
        public static User CurrentUser { get; set; }

        public static Action UpdateUI;
        public static Action UpdateMessages;


        public static FileServiceClient FileServiceClient { get; set; }

        static GlobalBase()
        {
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
            uiInfos.ForEach(item => loadPicture(userServiceClient, item));
        }

        public static void loadPicture(UserServiceClient userServiceClient, UiInfo uiInfos)
        {
            FileService.ChatFile chatFile = FileServiceClient.getChatFileById(uiInfos.ImageId);

            if (chatFile?.Source?.Length > 0)
            {
                MemoryStream memstr = new MemoryStream(chatFile.Source);
                Dispatcher.CurrentDispatcher.Invoke(() => { uiInfos.UiImage = Image.FromStream(memstr); });
            }
            else
            {
                Dispatcher.CurrentDispatcher.Invoke(() => { uiInfos.UiImage = ImageHelper.GetDefImage(); });
            }
        }

        public static Image getUsersAvatar(User user)
        {
            FileService.ChatFile chatFile = FileServiceClient.getChatFileById(user.ImageId);
            Image usersImage = null;

            if (chatFile?.Source?.Length > 0)
            {
                MemoryStream memstr = new MemoryStream(chatFile.Source);
                usersImage = Image.FromStream(memstr);
            }
            else
            {
                usersImage = ImageHelper.GetDefImage();
            }

            return usersImage;
        }
    }
}