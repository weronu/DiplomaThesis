using Domain.DomainClasses;
using Domain.DTOs;
using Domain.GraphClasses;
using UserDto = Domain.DTOs.UserDto;

namespace Thesis.Services.Interfaces
{
    public interface IGraphService
    {
        Graph<UserDto> FetchEmailsGraph(string connectionString);
        int FetchNodeIdByUserName(string name, string connectionString);
    }
}
