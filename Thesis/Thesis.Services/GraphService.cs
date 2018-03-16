using System.Collections.Generic;
using Domain.DTOs;
using Domain.GraphClasses;
using Repository.MSSQL.Interfaces;
using Thesis.Services.Interfaces;
using static Repository.MSSQL.UnitOfWorkFactory;


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
            
            using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
            {
                HashSet<Edge<UserDto>> edges = uow.GraphRepo.ExtractEdgesFromConversation();
                graph.CreateGraph(edges);
            }

            return graph;
        }

        public int FetchNodeIdByUserName(string name, string connectionString)
        {
            int nodeId;
            using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
            {
                nodeId = uow.UserRepo.GetNodeIdByUserName(name);
            }
            return nodeId;
        }

        public void ImportXMLFile(string pathToFile, string connectionString)
        {
            using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
            {
                uow.GraphRepo.ClearDatabaseData();
                uow.GraphRepo.ImportXmlFile(pathToFile);
            }
        }
    }
}
