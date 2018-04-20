using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.DTOs;
using Domain.GraphClasses;

namespace Repository.MSSQL.Interfaces
{
    public interface IGraphRepository
    {
        /// <summary>
        /// Extracts all vertices from database.
        /// </summary>
        HashSet<Node<UserDto>> ExtractNodesFromDatabase();
        HashSet<Node<UserDto>> ExtractNodesFromEdges(HashSet<Edge<UserDto>> edges);
        HashSet<Node<UserDto>> ExtractNodesFromConversations();
        HashSet<Edge<UserDto>> ExtractEdgesFromConversation();
        void ImportXmlFile(string pathToFile);
        void ExtractConversations();
        void ClearDatabaseData();
        HashSet<Edge<UserDto>> ExtractEdgesFromConversation(HashSet<ConversationEmails> conversationEmails);
        List<BrokerageDto> GetTopTenBrokers(HashSet<Node<UserDto>> nodes);
        NetworkStatisticsDto GetEmailNetworkStatistics();
    }
}
