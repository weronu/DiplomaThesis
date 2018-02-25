using System.Collections.Generic;
using Domain.DTOs;
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

        public Graph<UserDto> FetchEmailsGraph(string connectionString)
        {
            Graph<UserDto> graph = new Graph<UserDto>();
            
            using (IUnitOfWork uow = Repository.MSSQL.UnitOfWorkFactory.CreateUnitOfWork(connectionString))
            {
                HashSet<Edge<UserDto>> edges = uow.GraphRepo.ExtractEdgesFromConversation();
                graph.CreateGraph(edges);
            }

            return graph;
        }

        public int FetchNodeIdByUserName(string name, string connectionString)
        {
            int nodeId;
            using (IUnitOfWork uow = Repository.MSSQL.UnitOfWorkFactory.CreateUnitOfWork(connectionString))
            {
                nodeId = uow.UserRepo.GetNodeIdByUserName(name);
            }
            return nodeId;
        }
    }
}
