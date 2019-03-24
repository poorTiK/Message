using System.ServiceModel;
using ServerWCF.Contracts;

namespace ServerWCF.Services
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class PhotoService : IPhotoService
    {
        public byte[] GetPhotoById(int id)
        {
                return null;
        }

        public void SetPhotoById(int id, byte[] photoBytes)
        {

        }

        public byte[] GetFileByMessageId(int messageId)
        {
            return null;
        }

        public void SetFileToMessage(int messageId, byte[] file)
        {

        }
    }
}
