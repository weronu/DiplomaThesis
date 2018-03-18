using Domain.GraphClasses;
using Thesis.Services.ResponseTypes;
using UserDto = Domain.DTOs.UserDto;

namespace Thesis.Services.Interfaces
{
    public interface IGraphService
    {
        FetchItemServiceResponse<Graph<UserDto>> FetchEmailsGraph(string connectionString);
        int FetchNodeIdByUserName(string name, string connectionString);
        void ImportXMLFile(string pathToFile, string connectionString);
    }
}
