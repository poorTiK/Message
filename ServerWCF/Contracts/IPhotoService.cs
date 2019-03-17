using System.ServiceModel;

namespace ServerWCF.Contracts
{
    [ServiceContract]
    public interface IPhotoService
    {
        [OperationContract]
        byte[] GetPhotoById(int id);

        [OperationContract]
        void SetPhotoById(int id, byte[] photoBytes);

        [OperationContract]
        byte[] GetFileByMessageId(int messageId);

        [OperationContract]
        void SetFileToMessage(int messageId, byte[] file);
    }
}
