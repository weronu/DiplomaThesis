using System.Data.Entity;
using Domain.DomainClasses;

namespace Repository
{
    public class ThesisDbContext : DbContext
    {
        public ThesisDbContext() : base("ThesisDatabase")
        {

        }

        public DbSet<EmailMessage> EmailMessagess { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EmailRecipient> Recipients { get; set; }

    }
}



