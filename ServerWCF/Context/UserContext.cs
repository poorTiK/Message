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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>()
            //  .HasMany(user => user.Contacts)
            //  .WithRequired(contact => contact.UserOwned)
            //  .Map(config =>
            //  {
            //      config.MapKey("UserOwnedId");
            //      config.ToTable("Contacts");
            //  });

            //modelBuilder.Entity<User>()
            //  .HasMany(user => user.Owners)
            //  .WithRequired(contact => contact.UserOwner)
            //  .Map(config =>
            //  {
            //      config.MapKey("UserOwnerId");
            //      config.ToTable("Contacts");
            //  });

            //modelBuilder
            //    .Entity<User>()
            //    .HasMany(user => user.Contacts)
            //    .WithRequired()
            //    .Map(config =>
            //    {
            //        config.MapKey("UserOwnerId");
            //    });
        }
    }
}
