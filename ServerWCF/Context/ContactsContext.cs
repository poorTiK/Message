using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Context
{
    public class ContactsContext : DbContext
    {
        public ContactsContext() : base("SchoolDB-DataAnnotations")
        {
        }

        //public ContactsContext(DbSet<User> owners, DbSet<User> owned)
        //{
        //    Owners = owners;
        //    Owned = owned;
        //}

        public DbSet<User> Owners { get; set; }
        public DbSet<User> Owned { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
