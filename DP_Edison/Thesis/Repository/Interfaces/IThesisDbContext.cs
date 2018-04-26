using System;
using System.Data.Entity;
using Domain.DomainClasses;

namespace Repository.MSSQL.Interfaces
{
    public interface IThesisDbContext : IDisposable
    {
        int SaveChanges();

        DbSet<EmailMessage> EmailMessagess { get; set; }
        DbSet<User> Users { get; set; }
    }
}
