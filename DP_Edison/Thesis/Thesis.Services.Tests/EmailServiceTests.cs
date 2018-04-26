using Domain.DTOs;
using Domain.GraphClasses;
using NUnit.Framework;
using Thesis.Services.Interfaces;
using Thesis.Services.ResponseTypes;
using Thesis.Services.Tests.Base;

namespace Thesis.Services.Tests
{
    public class EmailServiceTests : IntegrationBase
    {
        [Test]
        public void FetchEmailGraphTest()
        {
            IGraphService service = new GraphService(UnitOfWorkFactory);

            const string connectionString = "GLEmailsDatabase";
            FetchItemServiceResponse<Graph<UserDto>> fetchEmailsGraph = service.FetchEmailsGraph(connectionString);
            Assert.IsNotEmpty(fetchEmailsGraph.Item.Edges);
        }
    }
}
