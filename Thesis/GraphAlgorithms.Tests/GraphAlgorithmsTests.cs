using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.GraphClasses;
using NUnit.Framework;
using Graph.Algorithms;
using Repository.MSSQL;
using Repository.MSSQL.Interfaces;
using Repository.MSSQL.Tests.Integration;

namespace GraphAlgorithms.Tests
{
    public class GraphAlgorithmsTests : DevBase
    {
        [Test]
        public void RunApp_Test()
        {
            string connectionString = "";
            Graph<User> graph = new Graph<User>();
            GraphAlgorithm<User> algorithms = new GraphAlgorithm<User>(graph);
            GraphRoleDetection<User> roleDetection = new GraphRoleDetection<User>(graph, algorithms);


            HashSet<Edge<User>> edges;
            using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
            {
                edges = uow.GraphRepo.ExtractEdgesFromDatabase();
            }
            foreach (Edge<User> edge in edges)
            {
                graph.CreateGraph(edge);
            }


            graph.GetEdgesCount();
            HashSet<ShortestPathSet<User>> shortestPaths = algorithms.GetAllShortestPathsInGraph(graph.Nodes);

            //setting closeness centrality
            algorithms.SetClosenessCentralityForEachNode(shortestPaths);

            //setting closeness centrality for community
            algorithms.SetClosenessCentralityForEachNodeInCommunity(shortestPaths);

            //community closeness centrality mean and standart deviation
            algorithms.SetMeanClosenessCentralityForEachCommunity();
            algorithms.SetStandartDeviationForClosenessCentralityForEachCommunity();

            //cPaths for nCBC measure
            HashSet<ShortestPathSet<User>> cPaths = algorithms.CPaths(shortestPaths);

            //setting nCBC for each node
            algorithms.SetNCBCForEachNode(cPaths);

            //setting DSCount for each node
            algorithms.SetDSCountForEachNode(cPaths);

            roleDetection.ExtractOutsiders();
            roleDetection.ExtractLeaders();
            roleDetection.ExtractOutermosts();

            //sorting nodes by their mediacy score
            HashSet<Node<User>> sortedNodes = algorithms.OrderNodesByMediacyScore();
            roleDetection.ExtractMediators(sortedNodes);
        }
    }
}
