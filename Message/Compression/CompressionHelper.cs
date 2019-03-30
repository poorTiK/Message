namespace Message.Compression
{
    public static class CompressionHelper
    {
        private static readonly long imageCompressionLevel = 50;
        private static readonly int imageCompressingBarrier = 1000000;

        private static ICompressStrategy CompressStrategy = new GZipCompressStrategy();

        public static byte[] CompressFile(byte[] data)
        {
            SetStrategyForFiles();
            return CompressStrategy.Compress(data);
        }

        public static byte[] DecompressFile(byte[] data)
        {
            SetStrategyForFiles();
            return CompressStrategy.Decompress(data);
        }

        public static byte[] CompressImage(byte[] imageData)
        {
            if (imageData.Length > imageCompressingBarrier)
            {
                SetStrategyForImages();
                imageData = CompressStrategy.Compress(imageData);
            }
            return imageData;
        }

        private static void SetStrategyForFiles()
        {
            CompressStrategy = new GZipCompressStrategy();
        }

        private static void SetStrategyForImages()
        {
            CompressStrategy = new ImageCompressStrategy(imageCompressionLevel);
        }
    }
}