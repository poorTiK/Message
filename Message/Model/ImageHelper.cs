using System.Drawing;
using System.IO;

namespace Message.Model
{
    public static class ImageHelper
    {
        public static byte[] GetDefImageBytes()
        {
            using (var ms = new MemoryStream())
            {
                var defImg = Image.FromFile(@"../../Resources/DefaultPicture.jpg");
                defImg.Save(ms, defImg.RawFormat);
                return ms.ToArray();
            }
        }

        public static Image GetDefImage()
        {
            return Image.FromFile(@"../../Resources/DefaultPicture.jpg");
        }

        public static byte[] GetImageBytes(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        internal static Image GetDefGroupImage()
        {
            return Image.FromFile(@"../../Resources/group.png");
        }
    }
}