using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Context
{
    public class ContactContext : DbContext
    {
        public ContactContext() : base("DbConnection") { }

        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Contact>()
            //.HasRequired(it => )
            //.WithMany()
            //.Map(m => m.MapKey("tree_id"));

            //modelBuilder.Entity<Contact>()
            //    .HasRequired(pt => pt.UserOwner)
            //    .WithMany(p => p.Contacts)
            //    .HasForeignKey(pt => pt.UserOwnerId);

            //modelBuilder.Entity<Contact>()
            //    .HasRequired(pt => pt.UserOwned)
            //    .WithMany(p => p.Contacts)
            //    .HasForeignKey(pt => pt.UserOwnedId);

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
