using Repository.MSSQL.Interfaces;
using Thesis.Services.Interfaces;

namespace Thesis.Services
{
    public class ServiceFactory : IServiceFactory
    {
        private static IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IThesisObjectFactory _objectFactory;

        public ServiceFactory(IUnitOfWorkFactory unitOfWorkFactory, IThesisObjectFactory objectFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _objectFactory = objectFactory;
        }

        public T GetService<T>(params object[] args) where T : class
        {
            object[] allArgs = new object[args.Length + 1];
            allArgs[0] = _unitOfWorkFactory;
            args.CopyTo(allArgs, 1);

            return _objectFactory.Get<T>(allArgs);
        }
    }
}
