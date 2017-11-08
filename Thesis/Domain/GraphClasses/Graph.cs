using System.Collections.Generic;
using System.Linq;
using Domain.DomainClasses;
using Domain.Enums;

namespace Domain.GraphClasses
{
    public class Graph<T> where T: DomainBase
    {
        public HashSet<Vertex<T>> Vertices { get; set; }
        public HashSet<Edge<T>> Edges { get; set; }
        public Dictionary<int, HashSet<Vertex<T>>> GraphSet { get; set; }
        public HashSet<Community<T>> Communities { get; set; }


        /// <summary>
        /// Constructor for the class Graph initializing
        /// </summary>
        public Graph()
        {
            Edges = new HashSet<Edge<T>>();
            Vertices = new HashSet<Vertex<T>>();
            GraphSet = new Dictionary<int, HashSet<Vertex<T>>>();
            Communities = new HashSet<Community<T>>();
        }

        /// <summary>
        /// Add vertex to the Graph
        /// </summary>
        public void AddVertex(Vertex<T> vertex)
        {
            if (Vertices.All(x => x.Id != vertex.Id))
                Vertices.Add(vertex);
        }

        /// <summary>
        /// Add edge to the Graph
        /// </summary>
        public void AddEdge(Edge<T> edge)
        {
            if (!Edges.Contains((Edge<T>) edge))
                Edges.Add(edge);

            if (!GraphSet.ContainsKey(edge.Vertex1.Id))
                GraphSet.Add(edge.Vertex1.Id, new HashSet<Vertex<T>>());

            if (!GraphSet.ContainsKey(edge.Vertex2.Id))
                GraphSet.Add(edge.Vertex2.Id, new HashSet<Vertex<T>>());


            if (GraphSet.ContainsKey(edge.Vertex1.Id))
                GraphSet[edge.Vertex1.Id].Add(edge.Vertex2);
            if (GraphSet.ContainsKey(edge.Vertex2.Id))
                GraphSet[edge.Vertex2.Id].Add(edge.Vertex1);

        }

        public void CreateGraph(Edge<T> edge)
        {
            AddVertex(edge.Vertex1);
            AddVertex(edge.Vertex2);

            AddEdge(edge);
        }

        /// <summary>
        /// Sets degree for each vertex in a graph
        /// </summary>
        public void SetDegrees()
        {
            foreach (Vertex<T> vertex in Vertices)
                vertex.Degree = GetDegree(vertex);
        }

        /// <summary>
        /// Returns all vertices with a Leader role
        /// </summary>
        public List<Vertex<T>> GetLeaders()
        {
            return Vertices.Select(x => x).Where(x => x.Role == Role.Leader).ToList();
        }

        /// <summary>
        /// Returns all vertices with Outsider role
        /// </summary>
        public List<Vertex<T>> GetOutsiders()
        {
            return Vertices.Select(x => x).Where(x => x.Role == Role.Outsider).ToList();
        }

        /// <summary>
        /// Returns all vertices with a Outermost role
        /// </summary>
        public List<Vertex<T>> GetOutermosts()
        {
            return Vertices.Select(x => x).Where(x => x.Role == Role.Outermost).ToList();
        }

        /// <summary>
        /// Returns all vertices with a Mediator role
        /// </summary>
        public List<Vertex<T>> GetMediators()
        {
            return Vertices.Select(x => x).Where(x => x.Role == Role.Mediator).ToList();
        }

        /// <summary>
        /// Get community by id
        /// </summary>
        public Community<T> GetCommunityById(int communityId)
        {
            return Communities.FirstOrDefault(i => i.Id == communityId);
        }

        /// <summary>
        /// Computes degree for a vertex
        /// </summary>
        public int GetDegree(Vertex<T> vertex)
        {
            return GetAdjacentVertices(vertex).Count;
        }

        /// <summary>
        /// Adjacent vertices of a vertex
        /// </summary>
        public HashSet<Vertex<T>> GetAdjacentVertices(Vertex<T> vertex)
        {
            return GraphSet[vertex.Id];
        }

        /// <summary>
        /// Count of all vertices in a graph
        /// </summary>
        public int GetVerticesCount()
        {
            return Vertices.Count;
        }

        /// <summary>
        /// Count of all vertices in a graph
        /// </summary>
        public int GetEdgesCount()
        {
            return Edges.Count;
        }

        /// <summary>
        /// Gets vertex by its id
        /// </summary>
        public Vertex<T> GetVertexById(int vertices)
        {
            return Vertices.FirstOrDefault(i => i.Id == vertices);
        }

        /// <summary>
        /// Returns maximal degree of vertices in a graph
        /// </summary>
        public int GetDegreeMax()
        {
            return Vertices.Select(x => x.Degree).Max();
        }

        /// <summary>
        /// Sorting vertices by community id
        /// </summary>
        public IOrderedEnumerable<Vertex<T>> SortNodesByCommunity()
        {
            return Vertices.OrderByDescending(x => x.Community.Id);
        }

        /// <summary>
        /// Returns mean degree
        /// </summary>
        public int GetDegreeMean()
        {
            var sum = Vertices.Sum(vertex => vertex.Degree);
            return sum / Vertices.Count;
        }

        /// <summary>
        /// Returns maximal closeness centrality
        /// </summary>
        public double GetClosenessCentralityMax()
        {
            return Vertices.Select(x => x.ClosenessCentrality).ToList().Max();
        }

        /// <summary>
        /// Get maximal mesiacy score
        /// </summary>
        public double GetMediacyScoreMax()
        {
            return Vertices.Select(x => x.MediacyScore).ToList().Max();
        }

        /// <summary>
        /// Gets incident communities for a vertex
        /// </summary>
        public List<Community<T>> GetIncidentCommunitiesOfVertex(Vertex<T> vertex)
        {
            List<Community<T>> incidentCommunities = new List<Community<T>> { vertex.Community };
            HashSet<Vertex<T>> adjacentNodes = GetAdjacentVertices(vertex);
            foreach (var adjacent in adjacentNodes)
            {
                Vertex<T> adjacentNode = GetVertexById(adjacent.Id);
                if (!incidentCommunities.Contains(adjacentNode.Community))
                {
                    incidentCommunities.Add(adjacentNode.Community);
                }
            }
            return incidentCommunities;
        }
    }
}
