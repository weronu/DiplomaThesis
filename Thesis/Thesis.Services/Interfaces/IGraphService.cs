using Domain.GraphClasses;
using Thesis.Services.ResponseTypes;
using UserDto = Domain.DTOs.UserDto;

namespace Thesis.Services.Interfaces
{
    public interface IGraphService
    {
        FetchItemServiceResponse<Graph<UserDto>> FetchEmailsGraph(string connectionString);
        FetchItemServiceResponse<int> FetchNodeIdByUserName(string name, string connectionString);
        ServiceResponse ImportXMLFile(string pathToFile, string connectionString);
    }
}
