using NUnit.Framework;
using Repository.MSSQL;

namespace Thesis.Services.Tests.Base
{
    public class IntegrationBase
    {
        protected UnitOfWorkFactory UnitOfWorkFactory;

        protected void CreateUnitOfWork()
        {
            UnitOfWorkFactory = new UnitOfWorkFactory("GLEmailsDatabase");
        }

        protected static ThesisDbContext GetThesisDbContext()
        {
            return new ThesisDbContext("GLEmailsDatabase");
        }


        [OneTimeSetUp]
        public void Setup()
        {
            CreateUnitOfWork();
        }

        protected void CleanUnitOfWork()
        {
            UnitOfWorkFactory = null;
        }
    }
}
