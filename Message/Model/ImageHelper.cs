using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Model
{
    public static class ImageHelper
    {
        public static byte[] GetDefImage()
        {
            using (var ms = new MemoryStream())
            {
                var defImg = Image.FromFile(@"../../Resources/DefaultPicture.jpg");
                defImg.Save(ms, defImg.RawFormat);
                return ms.ToArray();
            }
        }
    }
}
