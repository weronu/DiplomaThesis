using System;
using System.Collections.Generic;
using Domain.DTOs;
using Domain.GraphClasses;
using Repository.MSSQL.Interfaces;
using Thesis.Services.Interfaces;
using Thesis.Services.ResponseTypes;
using static Repository.MSSQL.UnitOfWorkFactory;


namespace Thesis.Services
{
    public class GraphService : ServiceBase, IGraphService
    {
        public GraphService(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {

        }

        public FetchItemServiceResponse<Graph<UserDto>> FetchEmailsGraph(string connectionString)
        {
            FetchItemServiceResponse<Graph<UserDto>> response = new FetchItemServiceResponse<Graph<UserDto>>();
            Graph<UserDto> graph = new Graph<UserDto>();
            
            using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
            {
                HashSet<Edge<UserDto>> edges = uow.GraphRepo.ExtractEdgesFromConversation();
                graph.CreateGraph(edges);
            }

            response.Item = graph;

            return response;
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
            try
            {
                using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
                {
                    uow.GraphRepo.ClearDatabaseData();
                    uow.GraphRepo.ImportXmlFile(pathToFile);
                    uow.GraphRepo.ExtractConversations();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Import of file failed with an error: {e}");
            }
        }
    }
}
