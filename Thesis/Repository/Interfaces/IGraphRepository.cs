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
        HashSet<Vertex<User>> GetVertices();

        /// <summary>
        /// Extracts all edges from database.
        /// </summary>
        HashSet<Edge<User>> GetEdges();
    }
}
