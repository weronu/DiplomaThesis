using Repository.MSSQL.Interfaces;

namespace Thesis.Services
{
    public class ServiceBase
    {
        public ServiceBase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
        }

        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }
    }
}
