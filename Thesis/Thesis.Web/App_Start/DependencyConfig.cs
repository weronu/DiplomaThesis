using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Http;
using Repository.MSSQL;
using Repository.MSSQL.Interfaces;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Thesis.Services;
using Thesis.Services.Interfaces;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

namespace Thesis.Web
{
    public class DependencyConfig
    {
        private static Container _container;

        public static Container Container => _container ?? (_container = CreateContainer());

        public static void Register(HttpConfiguration config)
        {
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(Container));
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorDependencyResolver(Container);
        }

        private static Container CreateContainer()
        {
            Container container = new Container();

            string connectionString = ConfigurationManager.ConnectionStrings["GLEmailsDatabase"].ConnectionString;

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            container.Register<IDbConnection>(() => new SqlConnection(connectionString), Lifestyle.Scoped);
            container.Register<IUnitOfWorkFactory>(() => new UnitOfWorkFactory(connectionString));
            container.Register<IThesisObjectFactory, ThesisObjectFactory>();
            container.Register<IServiceFactory, ServiceFactory>();
            container.Register<IGraphService>(() => new GraphService(container.GetInstance<IUnitOfWorkFactory>()));

            container.Verify();

            return container;
        }
    }
}