using System.Collections.Generic;
using Common;
using Domain.DomainClasses;
using FileHelper;

namespace Repository.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Repository.ThesisDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Repository.ThesisDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            //XmlParser xmlParser = new XmlParser();
            //HashSet<User> users = xmlParser.GetUsersFromXML(CommonMethods.GetAppSetting("datasetPath"));
            //foreach (User user in users)
            //{
            //    context.Users.AddOrUpdate(x => x.Email, user);
            //}
            //context.SaveChanges();

            //HashSet<EmailMessage> emailsFromXml = xmlParser.GetEmailsFromXML(CommonMethods.GetAppSetting("datasetPath"));
            //foreach (EmailMessage emailMessage in emailsFromXml)
            //{
            //    User sender = context.Users.FirstOrDefault(x => x.Email == emailMessage.SenderEmail);
            //    if (sender != null)
            //        context.EmailMessagess.AddOrUpdate(x => x.Id,
            //            new EmailMessage()
            //            {
            //                SenderId = sender.Id
            //            });
            //}
            //context.SaveChanges();
        }
    }
}
