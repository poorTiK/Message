using ServerWCF.Model;
using ServerWCF.Model.Contacts;
using ServerWCF.Model.Messages;
using System.Data.Entity;
using ServerWCF.Initializers;

namespace ServerWCF.Context
{
    internal class UserContext : DbContext
    {
        public UserContext() : base("DbConnection")
        {
            Database.SetInitializer(new UserDBInitializer());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BaseContact> Contacts { get; set; }
        public DbSet<BaseMessage> Messages { get; set; }
        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatFile> ChatFiles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseContact>()
                .HasRequired(s => s.UserOwner)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserToUserContact>()
                .HasRequired(s => s.UserOwned)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserToGroupContact>()
                .HasRequired(s => s.ChatGroup)
                .WithMany()
                .WillCascadeOnDelete(false);

            //files
            modelBuilder.Entity<BaseMessage>()
                .HasRequired(m => m.ChatFile)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasRequired(u => u.Image)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChatGroup>()
                .HasRequired(cg => cg.Image)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(false);
            ///////////////////////////
            modelBuilder.Entity<BaseMessage>()
                .HasRequired(s => s.Sender)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserMessage>()
                  .HasRequired(s => s.Receiver)
                  .WithMany()
                  .WillCascadeOnDelete(false);

        }
    }
}