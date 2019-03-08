using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Message.AdditionalItems
{
    internal class ByteToImageConverter : IValueConverter
    {
        public BitmapImage ConvertByteArrayToBitMapImage(byte[] imageByteArray)
        {
            BitmapImage img = new BitmapImage();
            using (MemoryStream memStream = new MemoryStream(imageByteArray))
            {
                img.StreamSource = memStream;
            }

            return img;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                BitmapImage img = new BitmapImage();
                if (value != null)
                {
                    img = this.ConvertByteArrayToBitMapImage(value as byte[]);
                }
                return img;
            }

            FileStream fileStream =
                new FileStream("..\\..\\Resources\\Test.jpg", FileMode.Open, FileAccess.Read);

            var defImg = new BitmapImage();
            defImg.BeginInit();
            defImg.StreamSource = fileStream;
            defImg.EndInit();

            return defImg;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(value as BitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }
    }
}