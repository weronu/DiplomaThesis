using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain.DomainClasses;
using Domain.Enums;
using Domain.GraphClasses;
using Graph.Algorithms;
using Repository.MSSQL;
using Repository.MSSQL.Interfaces;

namespace Application.Console
{
    public class GraphDemo
    {
        public void CreateGraphDemo()
        {
            #region CreatingBaseGraph

            const string path = @"C:\Users\veronika.uhrova\Desktop\Diplomka\results\";
            HashSet<Edge<User>> edges;
            HashSet<Node<User>> vertices;

            Stopwatch test01 = new Stopwatch();
            test01.Start();

            using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork("GLEmailsDatabase"))
            {
                edges = uow.GraphRepo.ExtractEdgesFromConversation();
                vertices = new HashSet<Node<User>>(uow.GraphRepo.ExtractVerticesFromConversations().OrderByDescending(x => x.Id).ToList());
            }

            Graph<User> graph = new Graph<User>
            {
                Nodes = new HashSet<Node<User>>(vertices.OrderByDescending(x => x.Id).ToList())
            };

            foreach (Edge<User> edge in edges)
            {
                graph.AddNode(edge.Node1);
                graph.AddNode(edge.Node2);
            }
            foreach (Edge<User> edge in edges)
            {
                graph.AddEdge(edge);
            }

            graph.SetDegrees();
            int maximalDegree = graph.GetMaximalDegree();
            int degreeMean = graph.GetDegreeMean();
            int edgesCount = graph.GetEdgesCount();
            int verticesCount = vertices.Count;

            System.Console.WriteLine(@"Count of vertices: {0}", verticesCount);
            System.Console.WriteLine(@"Count of edges: {0}", edgesCount);
            System.Console.WriteLine(@"Maximal degree: {0}", maximalDegree);
            System.Console.WriteLine(@"Average degree: {0}", degreeMean);
            System.Console.WriteLine(@"Creating gephi file ...");

            FileManager.FileWriter.CreateGephiFile(graph, path + "basegraph_allgraph_base.gml", false);

            #endregion

            #region EgoNetwork

            //creating ego network
            EgoNetwork egoNetwork = new EgoNetwork();

            HashSet<HashSet<Node<User>>> subGraphs = egoNetwork.FindConectedSubgraphs(graph);
            foreach (HashSet<Node<User>> subGraph in subGraphs)
            {
                foreach (Node<User> node in subGraph)
                {
                    System.Console.WriteLine(node.Id);
                }
                System.Console.WriteLine(@"-------------");
            }
            Node<User> egoNetworkCenter = graph.GetNodeById(349); // veronika uhrova
            HashSet<Node<User>> nodesWithMAximalDegreeInSubgraphsAximalDegreeInSubgraph = egoNetwork.GetNodesWithMaximalDegreeInSubgraphs(subGraphs, egoNetworkCenter);

            foreach (Node<User> node in nodesWithMAximalDegreeInSubgraphsAximalDegreeInSubgraph)
            {
                Edge<User> newEdge = new Edge<User>()
                {
                    Node1 = egoNetworkCenter,
                    Node2 = node
                };
                graph.AddEdge(newEdge);
            }

            FileManager.FileWriter.CreateGephiFile(graph, path + "egograph_allgraph_ego.gml", false);


            #endregion

            #region ExtractingCommunities

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            Dictionary<int, int> partition = LouvainCommunity.BestPartition(graph);
            System.Console.WriteLine(@"BestPartition: {0}", stopwatch.Elapsed);
            var communities = new Dictionary<int, List<int>>();
            foreach (KeyValuePair<int, int> kvp in partition)
            {
                List<int> nodeset;
                if (!communities.TryGetValue(kvp.Value, out nodeset))
                {
                    nodeset = communities[kvp.Value] = new List<int>();
                }
                nodeset.Add(kvp.Key);
            }
            System.Console.WriteLine(@"{0} communities found", communities.Count);
            int counter = 0;
            foreach (KeyValuePair<int, List<int>> kvp in communities)
            {
                System.Console.WriteLine(@"community {0}: {1} people", counter, kvp.Value.Count);
                counter++;
            }

            graph.SetCommunities(communities);

            #endregion

            #region ExtractingRoles

            GraphAlgorithm<User> algorithms = new GraphAlgorithm<User>(graph);
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

            GraphRoleDetection<User> roleDetection = new GraphRoleDetection<User>(graph, algorithms);
            roleDetection.ExtractOutsiders();
            roleDetection.ExtractLeaders();
            roleDetection.ExtractOutermosts();

            //sorting nodes by their mediacy score
            HashSet<Node<User>> sortedNodes = algorithms.OrderNodesByMediacyScore();
            roleDetection.ExtractMediators(sortedNodes);

            FileManager.FileWriter.CreateGephiFile(graph, path + "rolesgraph_allgraph_done.gml");


            #endregion


            IEnumerable<Node<User>> leaders = graph.Nodes.Where(x => x.Role == Role.Leader).ToList();
            IEnumerable<Node<User>> mediators = graph.Nodes.Where(x => x.Role == Role.Mediator).ToList();
            IEnumerable<Node<User>> outermosts = graph.Nodes.Where(x => x.Role == Role.Outermost).ToList();
            IEnumerable<Node<User>> outsiders = graph.Nodes.Where(x => x.Role == Role.Outsider).ToList();

            int leadersCount = leaders.Count();
            int mediatorsCount = mediators.Count();
            int outermostsCount = outermosts.Count();
            int outsidersCount = outsiders.Count();

            System.Console.WriteLine(@"Extracted {0} leaders.", leadersCount);
            System.Console.WriteLine(@"Extracted {0} mediators.", mediatorsCount);
            System.Console.WriteLine(@"Extracted {0} outermosts.", outermostsCount);
            System.Console.WriteLine(@"Extracted {0} outsiders.", outsidersCount);

            test01.Stop();
            System.Console.WriteLine(@"Done");
            System.Console.WriteLine($@"Execution time: {test01.Elapsed}");
            System.Console.ReadKey();
        }
    }
}
