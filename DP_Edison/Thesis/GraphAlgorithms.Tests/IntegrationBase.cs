using NUnit.Framework;
using Repository.MSSQL;

namespace GraphAlgorithms.Tests
{
    public class DevBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            CreateUnitOfWork();
        }

        protected UnitOfWorkFactory UnitOfWorkFactory;
        private static string _connectionString = "ThesisDbContext";


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
