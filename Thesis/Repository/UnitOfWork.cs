using System;
using System.Data.Entity;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private ThesisDbContext _dbContext;

        private IUserRepository _userRepository;
        private ICommonRepository _commonRepository;
        private IGraphRepository _graphRepository;

        private DbContextTransaction contextTransaction;

        public UnitOfWork(string connectionString)
        {
            _dbContext = new ThesisDbContext(connectionString);
        }
       
        public IUserRepository UserRepo
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_dbContext);
                }
                return _userRepository;
            }
        }

        public ICommonRepository CommonRepo
        {
            get
            {
                if (_commonRepository == null)
                {
                    _commonRepository = new CommonRepository(_dbContext);
                }
                return _commonRepository;
            }
        }

        public IGraphRepository GraphRepo
        {
            get
            {
                if (_graphRepository == null)
                {
                    _graphRepository = new GraphRepository(_dbContext);
                }
                return _graphRepository;
            }
        }

        public void SaveChanges()
        {

        }


        private bool disposed = false;


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    contextTransaction?.Dispose();
                    _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
