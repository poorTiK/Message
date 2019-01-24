using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Context
{
    public class MessageContext : DbContext
    {
        public MessageContext() : base("DbConnection")
        {
        }

        public DbSet<MessageT> Messages { get; set; }
    }
}
