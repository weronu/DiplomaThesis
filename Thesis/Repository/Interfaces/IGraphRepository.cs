using System.Collections.Generic;
using Domain.GraphClasses;
using User = Domain.DomainClasses.User;

namespace Repository.MSSQL.Interfaces
{
    public interface IGraphRepository
    {
        /// <summary>
        /// Extracts all vertices from database.
        /// </summary>
        HashSet<Node<User>> ExtractVerticesFromDatabase();
        HashSet<Node<User>> ExtractVerticesFromEdges(HashSet<Edge<User>> edges);
        HashSet<Node<User>> ExtractVerticesFromConversations();
        HashSet<Edge<Domain.DTOs.UserDto>> ExtractEdgesFromConversation();
        void ImportXmlFile(string pathToFile);
        List<int> GetConversations();
        void ExtractConversations();
        void ClearDatabaseData();
    }
}
