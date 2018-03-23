using System;
using System.Collections.Generic;
using Domain.DTOs;
using Domain.GraphClasses;
using Graph.Algorithms;
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

        public FetchItemServiceResponse<Graph<UserDto>> DetectRolesInGraph(Graph<UserDto> graph)
        {
            FetchItemServiceResponse<Graph<UserDto>> response = new FetchItemServiceResponse<Graph<UserDto>>();

            try
            {
                if (graph.Communities.Count == 0)
                {
                    throw new Exception("You have to find communities first!");
                }

                GraphAlgorithm<UserDto> algorithms = new GraphAlgorithm<UserDto>(graph);
                HashSet<ShortestPathSet<UserDto>> shortestPaths = algorithms.GetAllShortestPathsInGraph(graph.Nodes);

                //setting closeness centrality
                algorithms.SetClosenessCentralityForEachNode(shortestPaths);

                //setting closeness centrality for community
                algorithms.SetClosenessCentralityForEachNodeInCommunity(shortestPaths);

                //community closeness centrality mean and standart deviation
                algorithms.SetMeanClosenessCentralityForEachCommunity();
                algorithms.SetStandartDeviationForClosenessCentralityForEachCommunity();

                //cPaths for nCBC measure
                HashSet<ShortestPathSet<UserDto>> cPaths = algorithms.CPaths(shortestPaths);

                //setting nCBC for each node
                algorithms.SetNCBCForEachNode(cPaths);

                //setting DSCount for each node
                algorithms.SetDSCountForEachNode(cPaths);

                GraphRoleDetection<UserDto> roleDetection = new GraphRoleDetection<UserDto>(graph, algorithms);
                roleDetection.ExtractOutsiders();
                roleDetection.ExtractLeaders();
                roleDetection.ExtractOutermosts();

                //sorting nodes by their mediacy score
                HashSet<Node<UserDto>> sortedNodes = algorithms.OrderNodesByMediacyScore();
                roleDetection.ExtractMediators(sortedNodes);

                response.Succeeded = true;
                response.Item = graph;
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                response.AddError($"Detecting roles fasiled with an error: {e.Message}");
            }

            return response;
        }
    }
}
