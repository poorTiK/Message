using LoggingSystem;
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
                using (var userContext = new UserContext())
                {
                    return userContext.ChatFiles.FirstOrDefault(f => f.Id == id);
                }
            }
            catch (Exception ex)
            {
                LogHelper.logger.ErrorException(string.Format("ex in {name}", GetType().Name), ex);
                return null;
            }
        }

        public ChatFile getChatFileByName(string name)
        {
            try
            {
                using (var userContext = new UserContext())
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
                using (var userContext = new UserContext())
                {
                    userContext.ChatFiles.Add(chatFile);
                    userContext.SaveChanges();
                    var id = userContext.ChatFiles.Max(ch => ch.Id);

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
                using (var userContext = new UserContext())
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