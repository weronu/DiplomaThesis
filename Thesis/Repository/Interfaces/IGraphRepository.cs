using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.GraphClasses;

namespace Repository.MSSQL.Interfaces
{
    public interface IGraphRepository
    {
        /// <summary>
        /// Extracts all vertices from database.
        /// </summary>
        HashSet<Vertex<User>> ExtractVerticesFromDatabase();

        /// <summary>
        /// Extracts all edges from database.
        /// </summary>
        HashSet<Edge<User>> ExtractEdgesFromDatabase();

        HashSet<Vertex<User>> ExtractVerticesFromEdges(HashSet<Edge<User>> edges);
        HashSet<Vertex<User>> ExtractVerticesFromConversations();
        void ExtractEdgesFromConversation();
    }
}
