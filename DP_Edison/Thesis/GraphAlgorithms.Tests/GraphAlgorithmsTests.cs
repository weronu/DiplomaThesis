using System.Collections.Generic;
using Domain.DTOs;
using Domain.GraphClasses;
using Graph.Algorithms;
using NUnit.Framework;
using Repository.MSSQL.Interfaces;

namespace GraphAlgorithms.Tests
{
    public class GraphAlgorithmsTests : DevBase
    {
        [Test]
        public void RunApp_Test()
        {
            Graph<UserDto> graph = new Graph<UserDto>();
            GraphAlgorithm<UserDto> algorithms = new GraphAlgorithm<UserDto>(graph);
            GraphRoleDetection<UserDto> roleDetection = new GraphRoleDetection<UserDto>(graph, algorithms);


            HashSet<Edge<UserDto>> edges;
            using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
            {
                edges = uow.GraphRepo.ExtractEdgesFromConversation();
            }
            foreach (Edge<UserDto> edge in edges)
            {
                graph.CreateGraph(edge);
            }


            graph.GetEdgesCount();
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

            roleDetection.ExtractOutsiders();
            roleDetection.ExtractLeaders();
            roleDetection.ExtractOutermosts();

            //sorting nodes by their mediacy score
            HashSet<Node<UserDto>> sortedNodes = algorithms.OrderNodesByMediacyScore();
            roleDetection.ExtractMediators(sortedNodes);
        }
    }
}
