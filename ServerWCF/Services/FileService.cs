using ServerWCF.Context;
using ServerWCF.Contracts;
using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Services
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class FileService : IFileService
    {
        public ChatFile getChatFileById(int id)
        {
            using (UserContext userContext = new UserContext())
            {
                return userContext.ChatFiles.FirstOrDefault(f => f.Id == id);
            }
        }

        public ChatFile getChatFileByName(string name)
        {
            using (UserContext userContext = new UserContext())
            {
                return userContext.ChatFiles.FirstOrDefault(f => f.Name == name);
            }
        }

        public int UploadFile(ChatFile chatFile)
        {
            using (UserContext userContext = new UserContext())
            {
                userContext.ChatFiles.Add(chatFile);
                userContext.SaveChanges();
                int id = userContext.ChatFiles.Max(ch => ch.Id);

                return id;
            }
        }
    }
}
