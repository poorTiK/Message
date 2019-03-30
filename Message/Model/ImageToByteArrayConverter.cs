using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Message.Model
{
    public static class ImageToByteArrayConverter
    {
        public static byte[] ImageToByteArray(BitmapImage imageIn)
        {
            Stream stream = imageIn.StreamSource;
            byte[] buffer = null;
            if (stream != null && stream.Length > 0)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }

            return buffer;
        }
    }
}