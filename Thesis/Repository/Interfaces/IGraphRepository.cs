using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.DTOs;
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

        /// <summary>
        /// Extracts all edges from database.
        /// </summary>
        HashSet<Edge<User>> ExtractEdgesFromDatabase();

        HashSet<Node<User>> ExtractVerticesFromEdges(HashSet<Edge<User>> edges);
        HashSet<Node<User>> ExtractVerticesFromConversations();
        HashSet<Edge<Domain.DTOs.UserDto>> ExtractEdgesFromConversation();
    }
}
