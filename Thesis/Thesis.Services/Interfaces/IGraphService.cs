using Domain.DomainClasses;
using Domain.GraphClasses;

namespace Thesis.Services.Interfaces
{
    public interface IGraphService
    {
        Graph<User> FetchEmailsGraph(string connectionString);
    }
}
