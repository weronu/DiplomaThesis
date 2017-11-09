using System;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private string _connectionString;

        public UnitOfWorkFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(_connectionString);
        }

        public static IUnitOfWork CreateUnitOfWork(string _connectionString)
        {
            return new UnitOfWork(_connectionString);
        }
    }
}
