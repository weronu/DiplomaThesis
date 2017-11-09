using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.GraphClasses;
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
                vertices = uow.GraphRepo.ExtractVerticesFromEdges(edges);
            }



            Graph<User> graph = new Graph<User>();

            foreach (Edge<User> edge in edges)
            {
                graph.AddVertex(edge.Vertex1);
                graph.AddVertex(edge.Vertex2);
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

            FileManager.FileWriter.CreateGephiFile(graph, "D:/graph.gml", false);

            System.Console.WriteLine(@"Done");
            System.Console.ReadKey();
        }

    }
}
