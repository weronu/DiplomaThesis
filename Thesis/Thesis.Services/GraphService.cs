using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.GraphClasses;
using Repository.MSSQL.Interfaces;
using Thesis.Services.Interfaces;


namespace Thesis.Services
{
    public class GraphService : ServiceBase, IGraphService
    {
        public GraphService(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {

        }

        public Graph<User> FetchEmailsGraph(string connectionString)
        {
            Graph<User> graph = new Graph<User>();
            
            using (IUnitOfWork uow = Repository.MSSQL.UnitOfWorkFactory.CreateUnitOfWork(connectionString))
            {
                HashSet<Edge<User>> edges = uow.GraphRepo.ExtractEdgesFromConversation();
                graph.CreateGraph(edges);
            }

            return graph;
        }
    }
}
