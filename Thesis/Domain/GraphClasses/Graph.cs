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

        public IEnumerable<Edge<T>> EdgesEnumerator
        {
            get
            {
                foreach (var entry1 in GraphSet)
                {
                    foreach (var entry2 in entry1.Value)
                    {
                        if (entry1.Key <= entry2.Id)
                        {
                            yield return new Edge<T>(GetVertexById(entry1.Key), GetVertexById(entry2.Id));
                        }
                    }
                }
            }
        }

        public Dictionary<int, HashSet<Vertex<T>>> GraphSet { get; set; }
        public HashSet<Community<T>> Communities { get; set; }

        public double Size
        {
            get { return Edges.Select(x => x.Weight).Sum(); }
        }

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
        /// Computes degree for a vertex
        /// </summary>
        public double LouvainDegree(Vertex<T> vertex)
        {
            List<Edge<T>> incidentEdges = GetIncidentEdges(vertex).ToList();

            double loop = incidentEdges.Where(x => x.SelfLoop == true).Select(x => x.Weight).Sum();
            double degree = incidentEdges.Sum(x => x.Weight) + loop;

            return degree;
        }

        public IEnumerable<Edge<T>> GetIncidentEdges(Vertex<T> vertex)
        {
            HashSet<Edge<T>> edges = new HashSet<Edge<T>>(Edges.Where(x => x.Vertex1.Id == vertex.Id || x.Vertex2.Id == vertex.Id).ToList());
            foreach (Edge<T> edge in edges)
            {
                Vertex<T> first = null;
                Vertex<T> second = null;
                
                if (vertex.Id == edge.Vertex1.Id)
                {
                    first = vertex;
                    second = edge.Vertex2;
                }
                else
                {
                    first = edge.Vertex2;
                    second = edge.Vertex1;
                }
                yield return new Edge<T>(first, second, edge.Weight);
            }
        }


        public void AddEdge(Edge<T> edge)
        {
            bool existsInGraph = !(Edges.Any((i => i.Vertex1.Id == edge.Vertex1.Id && i.Vertex2.Id == edge.Vertex2.Id)) || (Edges.Any(i => i.Vertex1.Id == edge.Vertex2.Id && i.Vertex2.Id == edge.Vertex1.Id)));
            if (existsInGraph)
            {
                Edges.Add(edge);
            }

            if (!GraphSet.ContainsKey(edge.Vertex1.Id))
            {
                GraphSet.Add(edge.Vertex1.Id, new HashSet<Vertex<T>>());
            }

            if (!GraphSet.ContainsKey(edge.Vertex2.Id))
            {
                GraphSet.Add(edge.Vertex2.Id, new HashSet<Vertex<T>>());
            }

            if (GraphSet.ContainsKey(edge.Vertex1.Id))
            {
                GraphSet[edge.Vertex1.Id].Add(edge.Vertex2);
            }

            if (GraphSet.ContainsKey(edge.Vertex2.Id))
            {
                GraphSet[edge.Vertex2.Id].Add(edge.Vertex1);
            }

            //Size += edge.Weight;
        }

        public void AddWeightedEdge(Edge<T> edge, double weight)
        {
            bool existsInGraph = !(Edges.Any((i => i.Vertex1.Id == edge.Vertex1.Id && i.Vertex2.Id == edge.Vertex2.Id)) || (Edges.Any(i => i.Vertex1.Id == edge.Vertex2.Id && i.Vertex2.Id == edge.Vertex1.Id)));
            if (existsInGraph)
            {
                Edges.Add(edge);
            }

            if (!GraphSet.ContainsKey(edge.Vertex1.Id))
            {
                GraphSet.Add(edge.Vertex1.Id, new HashSet<Vertex<T>>());
            }

            if (!GraphSet.ContainsKey(edge.Vertex2.Id))
            {
                GraphSet.Add(edge.Vertex2.Id, new HashSet<Vertex<T>>());
            }

            if (GraphSet.ContainsKey(edge.Vertex1.Id))
            {
                GraphSet[edge.Vertex1.Id].Add(edge.Vertex2);
            }

            if (GraphSet.ContainsKey(edge.Vertex2.Id))
            {
                GraphSet[edge.Vertex2.Id].Add(edge.Vertex1);
            }

            SetEdgeWeight(edge, weight);
            //Size += edge.Weight;
        }

        private void SetEdgeWeight(Edge<T> edge, double weight)
        {
            Edge<T> e = Edges.FirstOrDefault(x => x.Vertex1.Id == edge.Vertex1.Id && x.Vertex2.Id == edge.Vertex2.Id || x.Vertex1.Id == edge.Vertex2.Id && x.Vertex2.Id == edge.Vertex1.Id);
            if (e != null) e.Weight = e.Weight + weight;
        }

        /// <summary>
        /// Returns the weight of the edge between two vertices.
        /// </summary>
        /// <param name="vertex1">The first vertex.</param>
        /// <param name="vertex2">The second vertex.</param>
        /// <returns>The weight of the edge.</returns>
        public double EdgeWeight(Vertex<User> vertex1, Vertex<User> vertex2)
        {
            return Edges.Where(x => x.Vertex1.Id == vertex1.Id && x.Vertex2.Id == vertex2.Id).Select(x => x.Weight).FirstOrDefault();
        }

        /// <summary>
        /// Returns a list of incident edges for a node.
        /// </summary>
        /// <param name="vertex">The vertex from which the returned edges will be incident.</param>
        /// <returns>An enumeration of incident edges.</returns>
        //public HashSet<Edge<T>> GetIncidentEdges(Vertex<T> vertex)
        //{
        //    HashSet<Edge<T>> incidentEdges = new HashSet<Edge<T>>();
        //    HashSet<Vertex<T>> hashSet = GraphSet[vertex.Id];
        //    foreach (Vertex<T> vertex2 in hashSet)
        //    {
        //        Edge<T> edge = new Edge<T>
        //        {
        //            Vertex1 = vertex,
        //            Vertex2 = vertex2,
        //            Weight = 2
        //        };
        //        incidentEdges.Add(edge);
        //    }
            
        //    return incidentEdges;
        //}

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
        /// Produces the induced graph from the quotient described by the partition.  The partition is a dictionary from nodes to communities.
        /// The produced graph has nodes which are communities, and there is a link of weight w between communities if the sum of the weights of the links
        /// between their elements is w.
        /// </summary>
        /// <param name="partition">A dictionary where keys are graph nodes and values are the community to which the node belongs.</param>
        /// <returns>The quotient graph.</returns>
        public Graph<T> Quotient(Dictionary<int, int> partition)
        {
            Graph<T> ret = new Graph<T>();
            foreach (int com in partition.Values)
            {
                Vertex<T> vertex = new Vertex<T>
                {
                    Id = com
                };
                ret.AddVertex(vertex);
                //ret.AddNode(com);
            }
            List<Edge<T>> list = Edges.OrderBy(x => x.Vertex1.Id).ThenBy(x => x.Vertex2.Id).ToList();
            var l = list.Select(x => new {vertex1 = x.Vertex1.Id, vertex2 = x.Vertex2.Id, edgeweight = x.Weight}).ToList();
            foreach (Edge<T> edge in list)
            {
                int pom1 = partition[edge.Vertex1.Id];
                int pom2 = partition[edge.Vertex2.Id];
                int vertex1Id = pom1;
                int vertex2Id = pom2;
                if (pom1 > pom2)
                {
                    vertex1Id = pom2;
                    vertex2Id = pom1;
                }
                double edgeWeight = edge.Weight;

                Vertex<T> vertex1 = ret.Vertices.FirstOrDefault(x => x.Id == vertex1Id);
                Vertex<T> vertex2 = ret.Vertices.FirstOrDefault(x => x.Id == vertex2Id);

                int count = 0;
                if (vertex1 != null && vertex2 != null)
                {
                    Edge<T> edgeCreated = new Edge<T>
                    {
                        Vertex1 = vertex1,
                        Vertex2 = vertex2
                    };
                    ret.AddWeightedEdge(edgeCreated, edgeWeight);
                    count++;
                }               
            }

            var enume = ret.EdgesEnumerator.Select(x => new {vertex1 = x.Vertex1.Id, vertex2 = x.Vertex2.Id, edgeweight = x.Weight}).ToList();
            var edgesOrdered = ret.Edges.Select(x => new {vertex1 = x.Vertex1.Id, vertex2 = x.Vertex2.Id, edgeweight = x.Weight}).OrderBy(x => x.vertex1).ToList();
            return ret;
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
            var degree = GetAdjacentVertices(vertex).Count;
            return degree;
        }

        /// <summary>
        /// Computes degree for a vertex
        /// </summary>
        public double GetDegreeAsSumOfWeights(Vertex<T> vertex)
        {
            HashSet<Edge<T>> incidentEdges = new HashSet<Edge<T>>(GetIncidentEdges(vertex).ToList());
            var sum = incidentEdges.Select(x => x.Weight).Sum();
            return sum;
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
        public int GetMaximalDegree()
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

    public class EdgeComparer : IEqualityComparer<Edge<User>>
    {
        public bool Equals(Edge<User> x, Edge<User> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.Vertex1.Id == y.Vertex2.Id || x.Vertex2.Id == y.Vertex1.Id;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(Edge<User> edge)
        {
            //Get hash code for the Name field if it is not null.
            int Vertex1hash = edge.Vertex1 == null ? 0 : edge.Vertex1.Id.GetHashCode();
            int Vertex2hash = edge.Vertex2 == null ? 0 : edge.Vertex2.Id.GetHashCode();

            //Calculate the hash code for the product.
            return Vertex1hash ^ Vertex2hash;
        }

    }
}
