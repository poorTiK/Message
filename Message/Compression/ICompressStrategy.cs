namespace Message.Compression
{
    public interface ICompressStrategy
    {
        byte[] Compress(byte[] data);

        byte[] Decompress(byte[] data);
    }
}