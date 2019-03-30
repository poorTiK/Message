using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Message.Compression
{
    public class ImageCompressStrategy : ICompressStrategy
    {
        private long compressionLevel;

        public ImageCompressStrategy(long compressionLevel)
        {
            this.compressionLevel = compressionLevel;
        }

        public byte[] Compress(byte[] data)
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            using (var inStream = new MemoryStream(data))
            using (var outStream = new MemoryStream())
            {
                Image image = Image.FromStream(inStream);

                if (jpgEncoder == null)
                {
                    image.Save(outStream, ImageFormat.Jpeg);
                }
                else
                {
                    System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;

                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(qualityEncoder, compressionLevel);

                    image.Save(outStream, jpgEncoder, encoderParameters);
                }

                return outStream.ToArray();
            }
        }

        public byte[] Decompress(byte[] data)
        {
            throw new NotImplementedException();
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            foreach (var codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
    }
}