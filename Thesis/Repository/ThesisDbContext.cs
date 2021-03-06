﻿using System.Data.Entity;
using Domain.DomainClasses;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class ThesisDbContext : DbContext, IThesisDbContext
    {
        public ThesisDbContext(string connectionString) : base(connectionString)
        {

        }

        public ThesisDbContext() 
        {

        }

        public DbSet<EmailMessage> EmailMessagess { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<UserEmail> UserEmails { get; set; }
    }
}



