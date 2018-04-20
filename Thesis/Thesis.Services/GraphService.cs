using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.DomainClasses;
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
                response.Error = ($"Import of file failed with an error: {e.Message}");

                if (e.InnerException != null)
                {
                    response.Error = ($"Additional error: {e.InnerException.Message}");
                }
            }

            return response;
        }

        public FetchItemServiceResponse<Graph<UserDto>> FetchEmailsGraph(string connectionString, DateTime fromDate, DateTime toDate)
        {
            FetchItemServiceResponse<Graph<UserDto>> response = new FetchItemServiceResponse<Graph<UserDto>>();
            try
            {
                Graph<UserDto> graph = new Graph<UserDto>();

                using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
                {
                    HashSet<ConversationEmails> conversationEmails = uow.ConvRepo.ExtractConversationsFromDatabase(fromDate, toDate);

                    HashSet<Edge<UserDto>> edges = uow.GraphRepo.ExtractEdgesFromConversation(conversationEmails);
                    graph.CreateGraph(edges);
                }

                response.Succeeded = true;
                response.Item = graph;
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                response.Error = ($"Import of file failed with an error: {e.Message}");

                if (e.InnerException != null)
                {
                    response.Error = ($"Additional error: {e.InnerException.Message}");
                }
            }

            return response;
        }

        public FetchListServiceResponse<DateTime> FetchStartAndEndOfConversation(string connectionString)
        {
            FetchListServiceResponse<DateTime> response = new FetchListServiceResponse<DateTime>();
            try
            {
                using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
                {
                    DateTime dateOfFirstConversation = uow.ConvRepo.GetDateOfFirstConversation();
                    DateTime dateOfLastConversation = uow.ConvRepo.GetDateOfLastConversation();

                    response.Items.Add(dateOfFirstConversation);
                    response.Items.Add(dateOfLastConversation);
                    response.Succeeded = true;
                }
            }
            catch (Exception)
            {
                response.Succeeded = false;
                throw new Exception("Start and end of conversation was not found.");
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
                    response.Error = ("Node was not found.");
                }
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                response.Error = ($"Import of file failed with an error: {e.Message}");

                if (e.InnerException != null)
                {
                    response.Error = ($"Additional error: {e.InnerException.Message}");
                }
            }

            return response;
        }

        public FetchItemServiceResponse<Node<UserDto>> FetchNodeWithBiggestDegree(string connectionString, Graph<UserDto> graph)
        {
            FetchItemServiceResponse<Node<UserDto>> response = new FetchItemServiceResponse<Node<UserDto>>();
            try
            {
                Node<UserDto> node = graph.Nodes.OrderByDescending(x => x.Degree).First();
                response.Item = node;


                if (response.Item == null)
                {
                    response.Succeeded = false;
                    throw new Exception("Node was not found.");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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

                response.SuccessMessage = ("XML file was successfully imported.");
                response.Succeeded = true;
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                response.Error = ($"Import of file failed with an error: {e.Message}");

                if (e.InnerException != null)
                {
                    response.Error = ($"Additional error: {e.InnerException.Message}");
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
                throw new Exception(e.Message);
            }

            return response;
        }

        public FetchItemServiceResponse<Graph<UserDto>> CreateEgoNetwork(Graph<UserDto> graph, int egoNetworkCenterId)
        {
            FetchItemServiceResponse<Graph<UserDto>> response = new FetchItemServiceResponse<Graph<UserDto>>();

            try
            {
                EgoNetwork egoNetwork = new EgoNetwork();

                HashSet<HashSet<Node<UserDto>>> subGraphs = egoNetwork.FindConectedSubgraphs(graph);
                
                Node<UserDto> egoNetworkCenter = graph.GetNodeById(egoNetworkCenterId);
                if (egoNetworkCenter == null)
                {
                    throw new Exception("Ego center node was not found.");
                }
                HashSet<Node<UserDto>> nodesWithMAximalDegreeInSubgraphsAximalDegreeInSubgraph = egoNetwork.GetNodesWithMaximalDegreeInSubgraphs(subGraphs, egoNetworkCenter);

                foreach (Node<UserDto> node in nodesWithMAximalDegreeInSubgraphsAximalDegreeInSubgraph)
                {
                    Edge<UserDto> newEdge = new Edge<UserDto>()
                    {
                        Node1 = egoNetworkCenter,
                        Node2 = node
                    };
                    graph.AddEdge(newEdge);
                }

                graph.SetDegrees();

                double eiIndex = egoNetwork.GetEIIndex(graph, egoNetworkCenter);
                double effectiveSizeOfEgo = egoNetwork.GetEffectiveSizeOfEgo(graph, egoNetworkCenter);


                graph.Nodes.First(x => x.Id == egoNetworkCenter.Id).EIIndex = eiIndex;
                graph.Nodes.First(x => x.Id == egoNetworkCenterId).EffectiveSize = effectiveSizeOfEgo;


                response.Succeeded = true;
                response.Item = graph;
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                throw new Exception(e.Message);
            }
            return response;
        }

        public FetchItemServiceResponse<Graph<UserDto>> DetectBrokerageInGraph(Graph<UserDto> graph)
        {
            FetchItemServiceResponse<Graph<UserDto>> response = new FetchItemServiceResponse<Graph<UserDto>>();
            if (graph.Communities.Count == 0)
            {
                throw new Exception("You have to find communities first!");
            }

            try
            {
                foreach (Node<UserDto> nodeB in graph.Nodes)
                {
                    HashSet<Node<UserDto>> adjacentNodes = graph.GetAdjacentNodes(nodeB);

                    nodeB.Brokerage = new Brokerage();
                    foreach (Node<UserDto> nodeA in adjacentNodes)
                    {
                        foreach (Node<UserDto> nodeC in adjacentNodes)
                        {
                            if (graph.ExistEdgeBetweenNodes(nodeA, nodeC) || nodeA.Id == nodeC.Id)
                            {
                                continue;
                            }

                            if (nodeA.CommunityId != nodeC.CommunityId && nodeA.CommunityId != nodeB.CommunityId && nodeC.CommunityId != nodeB.CommunityId)
                            {
                                nodeB.Brokerage.Liaison++;
                            }

                            if (nodeA.CommunityId == nodeC.CommunityId && nodeA.CommunityId != nodeB.CommunityId && nodeC.CommunityId != nodeB.CommunityId)
                            {
                                nodeB.Brokerage.Itinerant++;
                            }

                            if (nodeA.CommunityId == nodeB.CommunityId && nodeA.CommunityId == nodeC.CommunityId && nodeB.CommunityId == nodeC.CommunityId)
                            {
                                nodeB.Brokerage.Coordinator++;
                            }

                            if ((nodeA.CommunityId == nodeB.CommunityId && nodeA.CommunityId != nodeC.CommunityId && nodeB.CommunityId != nodeC.CommunityId)
                                || (nodeB.CommunityId == nodeC.CommunityId && nodeB.CommunityId != nodeA.CommunityId && nodeC.CommunityId != nodeA.CommunityId))
                            {
                                nodeB.Brokerage.Representative++;
                                nodeB.Brokerage.Gatepeeker++;
                            }
                        }
                    }
                }
                response.Item = graph;
                response.Succeeded = true;
                return response;
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                throw new Exception(e.Message);
            }
        }

        public FetchListServiceResponse<BrokerageDto> FetchTopTenBrokers(Graph<UserDto> graph, string connectionString)
        {
            FetchListServiceResponse<BrokerageDto> response = new FetchListServiceResponse<BrokerageDto>();
            try
            {
                List<BrokerageDto> topTenBrokers;
                using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
                {
                    topTenBrokers = uow.GraphRepo.GetTopTenBrokers(graph.Nodes);
                }

                response.Items = new HashSet<BrokerageDto>(topTenBrokers);
                response.Succeeded = true;
                return response;
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                throw new Exception(e.Message);
            }
        }

        public FetchListServiceResponse<DataPoint> FetchMostUsedEmailDomains(string connectionString, DateTime fromDate, DateTime toDate)
        {
            FetchListServiceResponse<DataPoint> response = new FetchListServiceResponse<DataPoint>();
            try
            {
                List<DataPoint> mostUsedEmailDomains;
                using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
                {
                    mostUsedEmailDomains = uow.UserRepo.GetTenMostUsedEmailDomains(fromDate, toDate);
                }

                response.Items = new HashSet<DataPoint>(mostUsedEmailDomains);
                response.Succeeded = true;
                return response;
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                throw new Exception(e.Message);
            }
        }

        public FetchItemServiceResponse<NetworkStatisticsDto> FetchEmailNetworkStatistics(string connectionString, DateTime fromDate, DateTime toDate)
        {
            FetchItemServiceResponse<NetworkStatisticsDto> response = new FetchItemServiceResponse<NetworkStatisticsDto>();
            try
            {
                NetworkStatisticsDto emailNetworkStatistics;
                using (IUnitOfWork uow = CreateUnitOfWork(connectionString))
                {
                    emailNetworkStatistics = uow.GraphRepo.GetEmailNetworkStatistics(fromDate, toDate);
                }

                response.Item = emailNetworkStatistics;
                response.Succeeded = true;
                return response;
            }
            catch (Exception e)
            {
                response.Succeeded = false;
                throw new Exception(e.Message);
            }
        }
    }
}
