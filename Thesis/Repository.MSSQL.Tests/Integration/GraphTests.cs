using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.GraphClasses;
using NUnit.Framework;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL.Tests.Integration
{
    public class GraphTests : IntegrationBase
    {
        [Test]
        public void Graph_Test()
        {
            HashSet<Edge<User>> edges;
            HashSet<Vertex<User>> vertices;

            using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
            {
                edges = uow.GraphRepo.ExtractEdgesFromConversation();
                vertices = uow.GraphRepo.ExtractVerticesFromEdges(edges);
            }

            Assert.IsNotNull(edges);
            Assert.IsNotEmpty(edges);

            Assert.IsNotNull(vertices);
            Assert.IsNotEmpty(vertices);

            Graph<User> graph = new Graph<User>();

            foreach (Vertex<User> vertex in vertices)
            {
                graph.AddVertex(vertex);
            }
            foreach (Edge<User> edge in edges)
            {
                graph.AddEdge(edge);
            }

            int maximalDegree = graph.GetMaximalDegree();
            int degreeMean = graph.GetDegreeMean();
            int edgesCount = graph.GetEdgesCount();
            int verticesCount = graph.GetVerticesCount();


        }
    }
}
