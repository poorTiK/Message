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
            using (UserContext db = new UserContext())
            {
                return db.Users
                    .FirstOrDefault(x => x.Id == id)
                    ?.Avatar;
            }
        }

        public void SetPhotoById(int id, byte[] photoBytes)
        {
            using (UserContext db = new UserContext())
            {
                var user = db.Users
                    .FirstOrDefault(x => x.Id == id);

                user.Avatar = photoBytes;
                db.Users.AddOrUpdate(user);
                db.SaveChanges();
            }
        }

        public byte[] GetFileByMessageId(int messageId)
        {
            using (UserContext userContext = new UserContext())
            {
                return userContext.Messages.FirstOrDefault(m => m.Id == messageId) ?. Content;
            }
        }

        public void SetFileToMessage(int messageId, byte[] file)
        {
            using (UserContext userContext = new UserContext())
            {
                BaseMessage baseMessage = userContext.Messages.FirstOrDefault(m => m.Id == messageId);

                baseMessage.Content = file;
                userContext.Messages.AddOrUpdate(baseMessage);
                userContext.SaveChanges();
            }
        }
    }
}
