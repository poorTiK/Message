using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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
