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
        public static Action UpdateContactList;
        public static Action UpdateMessagesOnUI;
        public static Action UpdateProfileUi;
        public static Action ExitProgramm;
        public static Func<BaseMessage, bool> RemoveMessageOnUI;

        public static FileServiceClient FileServiceClient { get; set; }
        public static Func<BaseMessage, bool> AddMessageOnUi { get; internal set; }
        public static UiInfo SelectedContact { get; internal set; }
        public static bool IsMenuEnabled { get; internal set; }
        public static readonly long fileSizeConstraint = 524288000L;
        public static readonly string ImagePattern = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";

        //monitors
        public static object contactsMonitor = new object();

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
            var fs = new FileStream(path,
                FileMode.Open,
                FileAccess.Read);
            var br = new BinaryReader(fs);
            var numBytes = new FileInfo(path).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        public static string GetShortName(string fullPath)
        {
            var pathArr = fullPath.Split('\\');
            var fileName = pathArr.Last().ToString();

            return fileName;
        }

        public static void loadPictures(UserServiceClient userServiceClient, List<UiInfo> uiInfos)
        {
            lock (contactsMonitor)
            {
                uiInfos.ForEach(uiInfo => loadPicture(userServiceClient, uiInfo));
            }
        }

        public static void loadPicture(UserServiceClient userServiceClient, UiInfo uiInfos)
        {
            var chatFile = FileServiceClient.getChatFileById(uiInfos.ImageId);

            if (chatFile?.Source?.Length > 0)
            {
                var memstr = new MemoryStream(chatFile.Source);
                Dispatcher.CurrentDispatcher.Invoke(() => { uiInfos.UiImage = Image.FromStream(memstr); });
            }
            else
            {
                Dispatcher.CurrentDispatcher.Invoke(() => { uiInfos.UiImage = ImageHelper.GetDefImage(); });
            }
        }

        public static Image getUsersAvatar(User user)
        {
            var chatFile = FileServiceClient.getChatFileById(user.ImageId);
            Image usersImage = null;

            if (chatFile?.Source?.Length > 0)
            {
                var memstr = new MemoryStream(chatFile.Source);
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