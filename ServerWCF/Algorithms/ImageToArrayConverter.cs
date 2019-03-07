using System;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace Message.AdditionalItems
{
    public static class ImageToArrayConverter
    {
        public static byte[] ImageToByteArray(string path)
        {
            Image image = Image.FromFile(path);

            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
    }
}