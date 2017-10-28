using Domain.DomainClasses;
using NUnit.Framework;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL.Tests.Integration
{
    public class RepositoryTests : DevBase
    {

        [Test]
        public void User_RepositoryTest()
        {
            User user;
            using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
            {
                user = uow.CommonRepo.GetById<User>(1);
            }

            Assert.IsNotNull(user);
        }

        [Test]
        public void EmailMessage_RepositoryTest()
        {
            EmailMessage email;
            using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
            {
                email = uow.CommonRepo.GetById<EmailMessage>(1);
            }

            Assert.IsNotNull(email);
        }

        [Test]
        public void EmailRecipient_RepositoryTest()
        {
            EmailRecipient recipient;
            using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
            {
                recipient = uow.CommonRepo.GetById<EmailRecipient>(1);
            }

            Assert.IsNotNull(recipient);
        }
    }
}
