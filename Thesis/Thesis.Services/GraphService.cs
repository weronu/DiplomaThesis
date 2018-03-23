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
            try
            {
                Graph<UserDto> graph = new Graph<UserDto>();

                using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
                {
                    HashSet<Edge<UserDto>> edges = uow.GraphRepo.ExtractEdgesFromConversation();
                    graph.CreateGraph(edges);
                }

                response.Succeeded = true;
                response.Item = graph;
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                response.AddError($"Import of file failed with an error: {e.Message}");

                if (e.InnerException != null)
                {
                    response.AddError($"Additional error: {e.InnerException.Message}");
                }
            }

            return response;
        }

        public FetchItemServiceResponse<int> FetchNodeIdByUserName(string name, string connectionString)
        {
            FetchItemServiceResponse<int> response = new FetchItemServiceResponse<int>();
            try
            {

                using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
                {
                    int nodeId = uow.UserRepo.GetNodeIdByUserName(name);
                    response.Item = nodeId;
                }

                if (response.Item == 0)
                {
                    response.Succeeded = false;
                    response.AddError("Node was not found.");
                }
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                response.AddError($"Import of file failed with an error: {e.Message}");

                if (e.InnerException != null)
                {
                    response.AddError($"Additional error: {e.InnerException.Message}");
                }
            }

            return response;
        }

        public ServiceResponse ImportXMLFile(string pathToFile, string connectionString)
        {
            ServiceResponse response = new ServiceResponse();
            try
            {
                using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
                {
                    uow.GraphRepo.ClearDatabaseData();
                    uow.GraphRepo.ImportXmlFile(pathToFile);
                    uow.GraphRepo.ExtractConversations();
                }

                response.AddSuccessMessage("XML file was successfully imported.");
                response.Succeeded = true;
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                response.AddError($"Import of file failed with an error: {e.Message}");

                if (e.InnerException != null)
                {
                    response.AddError($"Additional error: {e.InnerException.Message}");
                }
            }

            return response;
        }
    }
}
