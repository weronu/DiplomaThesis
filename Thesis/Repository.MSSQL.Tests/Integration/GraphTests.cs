using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.GraphClasses;
using NUnit.Framework;
using Repository.MSSQL.Interfaces;

namespace Repository.MSSQL.Tests.Integration
{
    public class GraphTests : IntegrationBase
    {
        //[Test]
        //public void Graph_Test()
        //{
        //    HashSet<Edge<User>> edges;
        //    HashSet<Node<User>> nodes;

        //    using (IUnitOfWork uow = UnitOfWorkFactory.CreateUnitOfWork())
        //    {
        //        edges = uow.GraphRepo.ExtractEdgesFromConversation();
        //        nodes = uow.GraphRepo.ExtractVerticesFromEdges(edges);
        //    }

        //    Assert.IsNotNull(edges);
        //    Assert.IsNotEmpty(edges);

        //    Assert.IsNotNull(nodes);
        //    Assert.IsNotEmpty(nodes);

        //    Graph<User> graph = new Graph<User>();

        //    foreach (Node<User> node in nodes)
        //    {
        //        graph.AddNode(node);
        //    }
        //    foreach (Edge<User> edge in edges)
        //    {
        //        graph.AddEdge(edge);
        //    }

        //    int maximalDegree = graph.GetMaximalDegree();
        //    int degreeMean = graph.GetDegreeMean();
        //    int edgesCount = graph.GetEdgesCount();
        //    int nodescount = graph.GetVerticesCount();


        //}
    }
}
