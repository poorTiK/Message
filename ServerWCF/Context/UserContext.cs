using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Context
{
    class UserContext : DbContext
    {
        public UserContext() : base("DbConnection") { }

        public DbSet<User> Users { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>()
                .HasRequired(s => s.UserOwned)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Contact>()
                .HasRequired(s => s.UserOwner)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}
