using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message.Compression
{
    public interface ICompressStrategy
    {
        byte[] Compress(byte[] data);

        byte[] Decompress(byte[] data);
    }
}
