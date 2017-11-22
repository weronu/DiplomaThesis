using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain.DomainClasses;
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

            HashSet<Edge<User>> edges;
            HashSet<Vertex<User>> vertices;

            using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork("GLEmailsDatabase"))
            {
                edges = uow.GraphRepo.ExtractEdgesFromConversation();
                //vertices = uow.GraphRepo.ExtractVerticesFromEdges(edges);
                vertices = new HashSet<Vertex<User>>(uow.GraphRepo.ExtractVerticesFromConversations().OrderByDescending(x => x.Id).ToList());
            }

            Graph<User> graph = new Graph<User>();

            graph.Vertices = new HashSet<Vertex<User>>(vertices.OrderByDescending(x => x.Id).ToList());
            foreach (Edge<User> edge in edges)
            {
                graph.AddVertex(edge.Vertex1);
                graph.AddVertex(edge.Vertex2);
            }
            foreach (Edge<User> edge in edges)
            {
                graph.AddEdge(edge);
            }

            //FileManager.FileWriter.CreateFile(graph, "D:/graph.txt");
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

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            Dictionary<int, int> partition = LouvainCommunity.BestPartition(graph);
            System.Console.WriteLine(@"BestPartition: {0}", stopwatch.Elapsed);
            var communities = new Dictionary<int, List<int>>();
            foreach (var kvp in partition)
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
            foreach (var kvp in communities)
            {
                System.Console.WriteLine(@"community {0}: {1} people", counter, kvp.Value.Count);
                counter++;
            }
            System.Console.ReadLine();

            FileManager.FileWriter.CreateGephiFile(graph, "D:/graph.gml", false);

            System.Console.WriteLine(@"Done");
            System.Console.ReadKey();
        }


    }
}
