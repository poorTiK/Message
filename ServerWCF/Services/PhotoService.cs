using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.ServiceModel;
using ServerWCF.Context;
using ServerWCF.Contracts;
using ServerWCF.Model.Messages;

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
