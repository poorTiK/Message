﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Model
{
    public static class CompressionHelper
    {
        //public static byte[] Compress(byte[] data)
        //{
        //    MemoryStream output = new MemoryStream();
        //    using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
        //    {
        //        dstream.Write(data, 0, data.Length);
        //    }
        //    return output.ToArray();
        //}

        //public static byte[] Decompress(byte[] data)
        //{
        //    MemoryStream input = new MemoryStream(data);
        //    MemoryStream output = new MemoryStream();
        //    using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
        //    {
        //        dstream.CopyTo(output);
        //    }
        //    return output.ToArray();
        //}

        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,
                    CompressionMode.Compress, true))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return memory.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(data),
                CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }
}
