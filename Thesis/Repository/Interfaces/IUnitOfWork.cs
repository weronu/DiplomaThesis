using System;

namespace Repository.MSSQL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepo { get; }
        ICommonRepository CommonRepo { get; }
        IGraphRepository GraphRepo { get; }

        void SaveChanges();
    }
}
