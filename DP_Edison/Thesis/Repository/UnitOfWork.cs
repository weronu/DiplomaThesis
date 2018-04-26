using System;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ThesisDbContext _dbContext;

        private IUserRepository _userRepository;
        private ICommonRepository _commonRepository;
        private IGraphRepository _graphRepository;
        private IConversationRepository _convRepository;

        public UnitOfWork(string connectionString)
        {
            _dbContext = new ThesisDbContext(connectionString);
        }
       
        public IUserRepository UserRepo => _userRepository ?? (_userRepository = new UserRepository(_dbContext));

        public ICommonRepository CommonRepo => _commonRepository ?? (_commonRepository = new CommonRepository(_dbContext));

        public IGraphRepository GraphRepo => _graphRepository ?? (_graphRepository = new GraphRepository(_dbContext));

        public IConversationRepository ConvRepo => _convRepository ?? (_convRepository = new ConversationRepository(_dbContext));

        public void SaveChanges()
        {

        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
