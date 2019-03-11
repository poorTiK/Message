﻿using ServerWCF.Model;
using ServerWCF.Model.Contacts;
using ServerWCF.Model.Messages;
using System.Data.Entity;

namespace ServerWCF.Context
{
    internal class UserContext : DbContext
    {
        public UserContext() : base("DbConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BaseContact> Contacts { get; set; }
        public DbSet<BaseMessage> Messages { get; set; }
        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserToUserContact>()
                .HasRequired(s => s.UserOwned)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserToGroupContact>()
                .HasRequired(s => s.ChatGroup)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BaseContact>()
                .HasRequired(s => s.UserOwner)
                .WithMany()
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserMessage>()
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