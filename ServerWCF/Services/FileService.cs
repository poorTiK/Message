using ServerWCF.Context;
using ServerWCF.Contracts;
using ServerWCF.Model;
using System;
using System.Linq;
using System.ServiceModel;

namespace ServerWCF.Services
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class FileService : IFileService
    {
        public ChatFile getChatFileById(int id)
        {
            try
            {
                using (UserContext userContext = new UserContext())
                {
                    return userContext.ChatFiles.FirstOrDefault(f => f.Id == id);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ChatFile getChatFileByName(string name)
        {
            try
            {
                using (UserContext userContext = new UserContext())
                {
                    return userContext.ChatFiles.FirstOrDefault(f => f.Name == name);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int UploadFile(ChatFile chatFile)
        {
            try
            {
                using (UserContext userContext = new UserContext())
                {
                    userContext.ChatFiles.Add(chatFile);
                    userContext.SaveChanges();
                    int id = userContext.ChatFiles.Max(ch => ch.Id);

                    return id;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public ChatFile UpdateFileSource(int fileId, byte[] source)
        {
            try
            {
                using (UserContext userContext = new UserContext())
                {
                    var file = userContext.ChatFiles.FirstOrDefault(x => x.Id == fileId);
                    file.Source = source;
                    userContext.SaveChanges();

                    return file;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}