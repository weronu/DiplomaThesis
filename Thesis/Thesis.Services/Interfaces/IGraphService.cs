using System;
using Domain.DTOs;
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
        FetchItemServiceResponse<Graph<UserDto>> DetectRolesInGraph(Graph<UserDto> graph);
        FetchItemServiceResponse<Graph<UserDto>> FetchEmailsGraph(string connectionString, DateTime fromDate, DateTime toDate);
        FetchListServiceResponse<DateTime> FetchStartAndEndOfConversation(string connectionString);
        FetchItemServiceResponse<Node<UserDto>> FetchNodeWithBiggestDegree(string connectionString, Graph<UserDto> graph);
        FetchItemServiceResponse<Graph<UserDto>> DetectBrokerageInGraph(Graph<UserDto> graph);
        FetchListServiceResponse<BrokerageDto> FetchTopTenBrokers(Graph<UserDto> graph, string connectionString);
        FetchItemServiceResponse<Graph<UserDto>> CreateEgoNetwork(Graph<UserDto> graph, int egoNetworkCenterId);
    }
}
