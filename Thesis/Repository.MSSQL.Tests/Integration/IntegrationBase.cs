using NUnit.Framework;

namespace Repository.MSSQL.Tests.Integration
{
    public class IntegrationBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            CreateUnitOfWork();
        }

        protected UnitOfWorkFactory UnitOfWorkFactory;
        private static string _connectionString = "GLEmailsDatabase";


        protected void CreateUnitOfWork()
        {
            UnitOfWorkFactory = new UnitOfWorkFactory(_connectionString);
        }

        protected static ThesisDbContext GetOneTrackDbContext()
        {
            return new ThesisDbContext(_connectionString);
        }

        protected void CleanUnitOfWork()
        {
            UnitOfWorkFactory = null;
        }

    }
}
