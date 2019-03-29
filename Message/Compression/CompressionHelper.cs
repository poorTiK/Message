﻿using Message.Compression;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Compression
{
    public static class CompressionHelper
    {
        private static long imageCompressionLevel = 50;
        private static int imageCompressingBarrier = 1000000;

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
