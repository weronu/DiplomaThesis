using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.GraphClasses;
using NUnit.Framework;
using Thesis.Services.Interfaces;
using Thesis.Services.Tests.Base;

namespace Thesis.Services.Tests
{
    public class EmailServiceTests : IntegrationBase
    {
        [Test]
        public void FetchEmailGraphTest()
        {
            IGraphService service = new GraphService(UnitOfWorkFactory);

            string connectionString = "GLEmailsDatabase";
            Graph<User> fetchEmailsGraph = service.FetchEmailsGraph(connectionString);
            Assert.IsNotEmpty(fetchEmailsGraph.Edges);
        }
    }
}
