using ServerWCF.Model;
using System.ServiceModel;

namespace ServerWCF.Contracts
{
    [ServiceContract]
    public interface IFileService
    {
        [OperationContract]
        ChatFile getChatFileById(int id);

        [OperationContract]
        ChatFile getChatFileByName(string name);

        [OperationContract]
        int UploadFile(ChatFile chatFile);

        [OperationContract]
        ChatFile UpdateFileSource(int fileId, byte[] source);
    }
}