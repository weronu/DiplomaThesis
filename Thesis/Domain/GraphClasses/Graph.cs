using System.Collections.Generic;
using System.Linq;
using Domain.DomainClasses;
using Domain.Enums;

namespace Domain.GraphClasses
{
    public class Graph<T> where T: DomainBase
    {
        public HashSet<Node<T>> Nodes { get; set; }
        public HashSet<Edge<T>> Edges { get; set; }
        public Dictionary<int, HashSet<Node<T>>> GraphSet { get; set; }
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
            Nodes = new HashSet<Node<T>>();
            GraphSet = new Dictionary<int, HashSet<Node<T>>>();
            Communities = new HashSet<Community<T>>();
        }

        /// <summary>
        /// Add node to the Graph
        /// </summary>
        public void AddNode(Node<T> node)
        {
            if (Nodes.All(x => x.Id != node.Id))
            {
                Nodes.Add(node);
            }
        }

        /// <summary>
        /// Computes degree for a node
        /// </summary>
        public double LouvainDegree(Node<T> node)
        {
            List<Edge<T>> incidentEdges = GetIncidentEdges(node).ToList();

            double loop = incidentEdges.Where(x => x.SelfLoop == true).Select(x => x.Weight).Sum();
            double degree = incidentEdges.Sum(x => x.Weight) + loop;

            return degree;
        }

        /// <summary>
        /// Returns a list of incident edges for a node.
        /// </summary>
        /// <param name="node">The node from which the returned edges will be incident.</param>
        /// <returns>An enumeration of incident edges.</returns>
        public IEnumerable<Edge<T>> GetIncidentEdges(Node<T> node)
        {
            HashSet<Edge<T>> edges = new HashSet<Edge<T>>(Edges.Where(x => x.Node1.Id == node.Id || x.Node2.Id == node.Id).ToList());
            foreach (Edge<T> edge in edges)
            {
                Node<T> first;
                Node<T> second;
                
                if (node.Id == edge.Node1.Id)
                {
                    first = node;
                    second = edge.Node2;
                }
                else
                {
                    first = edge.Node2;
                    second = edge.Node1;
                }
                yield return new Edge<T>(first, second, edge.Weight);
            }
        }


        public void AddEdge(Edge<T> edge, double weight = 0)
        {
            bool existsInGraph = !(Edges.Any((i => i.Node1.Id == edge.Node1.Id && i.Node2.Id == edge.Node2.Id)) || (Edges.Any(i => i.Node1.Id == edge.Node2.Id && i.Node2.Id == edge.Node1.Id)));
            if (existsInGraph)
            {
                Edges.Add(edge);
            }

            if (!GraphSet.ContainsKey(edge.Node1.Id))
            {
                GraphSet.Add(edge.Node1.Id, new HashSet<Node<T>>());
            }

            if (!GraphSet.ContainsKey(edge.Node2.Id))
            {
                GraphSet.Add(edge.Node2.Id, new HashSet<Node<T>>());
            }

            if (GraphSet.ContainsKey(edge.Node1.Id))
            {
                GraphSet[edge.Node1.Id].Add(edge.Node2);
            }

            if (GraphSet.ContainsKey(edge.Node2.Id))
            {
                GraphSet[edge.Node2.Id].Add(edge.Node1);
            }
            SetEdgeWeight(edge, weight);
        }

        private void SetEdgeWeight(Edge<T> edge, double weight)
        {
            Edge<T> e = Edges.FirstOrDefault(x => x.Node1.Id == edge.Node1.Id && x.Node2.Id == edge.Node2.Id || x.Node1.Id == edge.Node2.Id && x.Node2.Id == edge.Node1.Id);
            if (e != null) e.Weight = e.Weight + weight;
        }

        /// <summary>
        /// Returns the weight of the edge between two vertices.
        /// </summary>
        /// <param name="node1">The first node.</param>
        /// <param name="node2">The second node.</param>
        /// <returns>The weight of the edge.</returns>
        public double EdgeWeight(Node<User> node1, Node<User> node2)
        {
            return Edges.Where(x => x.Node1.Id == node1.Id && x.Node2.Id == node2.Id).Select(x => x.Weight).FirstOrDefault();
        }

        public void CreateGraph(Edge<T> edge)
        {
            AddNode(edge.Node1);
            AddNode(edge.Node2);

            AddEdge(edge);
        }

        public void CreateGraph(HashSet<Edge<T>> edges)
        {
            foreach (Edge<T> edge in edges)
            {
                AddNode(edge.Node1);
                AddNode(edge.Node2);

                AddEdge(edge);
            }
        }

        /// <summary>
        /// Sets degree for each node in a graph
        /// </summary>
        public void SetDegrees()
        {
            foreach (Node<T> node in Nodes)
            {
                node.Degree = GetDegree(node);
            }
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
                Node<T> node = new Node<T>
                {
                    Id = com
                };
                ret.AddNode(node);
            }
            List<Edge<T>> edges = Edges.OrderBy(x => x.Node1.Id).ThenBy(x => x.Node2.Id).ToList();
            foreach (Edge<T> edge in edges)
            {
                int pom1 = partition[edge.Node1.Id];
                int pom2 = partition[edge.Node2.Id];
                int node1Id = pom1;
                int node2Id = pom2;
                if (pom1 > pom2)
                {
                    node1Id = pom2;
                    node2Id = pom1;
                }
                double edgeWeight = edge.Weight;

                Node<T> node1 = ret.Nodes.FirstOrDefault(x => x.Id == node1Id);
                Node<T> node2 = ret.Nodes.FirstOrDefault(x => x.Id == node2Id);

                if (node1 != null && node2 != null)
                {
                    Edge<T> edgeCreated = new Edge<T>
                    {
                        Node1 = node1,
                        Node2 = node2
                    };
                    ret.AddEdge(edgeCreated, edgeWeight);
                }               
            }
            return ret;
        }

        /// <summary>
        /// Returns all vertices with a Leader role
        /// </summary>
        public List<Node<T>> GetLeaders()
        {
            return Nodes.Select(x => x).Where(x => x.Role == Role.Leader).ToList();
        }

        /// <summary>
        /// Returns all vertices with Outsider role
        /// </summary>
        public List<Node<T>> GetOutsiders()
        {
            return Nodes.Select(x => x).Where(x => x.Role == Role.Outsider).ToList();
        }

        /// <summary>
        /// Returns all vertices with a Outermost role
        /// </summary>
        public List<Node<T>> GetOutermosts()
        {
            return Nodes.Select(x => x).Where(x => x.Role == Role.Outermost).ToList();
        }

        /// <summary>
        /// Returns all vertices with a Mediator role
        /// </summary>
        public List<Node<T>> GetMediators()
        {
            return Nodes.Select(x => x).Where(x => x.Role == Role.Mediator).ToList();
        }

        /// <summary>
        /// Get community by id
        /// </summary>
        public Community<T> GetCommunityById(int communityId)
        {
            return Communities.FirstOrDefault(i => i.Id == communityId);
        }

        /// <summary>
        /// Computes degree for a node
        /// </summary>
        public int GetDegree(Node<T> node)
        {
            var degree = GetAdjacentNodes(node).Count;
            return degree;
        }

        /// <summary>
        /// Computes degree for a node
        /// </summary>
        public double GetDegreeAsSumOfWeights(Node<T> node)
        {
            HashSet<Edge<T>> incidentEdges = new HashSet<Edge<T>>(GetIncidentEdges(node).ToList());
            double sum = incidentEdges.Select(x => x.Weight).Sum();
            return sum;
        }

        /// <summary>
        /// Adjacent vertices of a node
        /// </summary>
        public HashSet<Node<T>> GetAdjacentNodes(Node<T> node)
        {
            return GraphSet[node.Id];
        }

        /// <summary>
        /// Count of all vertices in a graph
        /// </summary>
        public int GetVerticesCount()
        {
            return Nodes.Count;
        }

        /// <summary>
        /// Count of all vertices in a graph
        /// </summary>
        public int GetEdgesCount()
        {
            return Edges.Count;
        }

        /// <summary>
        /// Gets node by its id
        /// </summary>
        public Node<T> GetNodeById(int vertices)
        {
            return Nodes.FirstOrDefault(i => i.Id == vertices);
        }

        /// <summary>
        /// Returns maximal degree of vertices in a graph
        /// </summary>
        public int GetMaximalDegree()
        {
            return Nodes.Select(x => x.Degree).Max();
        }

        /// <summary>
        /// Sorting vertices by community id
        /// </summary>
        public IOrderedEnumerable<Node<T>> SortNodesByCommunity()
        {
            return Nodes.OrderByDescending(x => x.Community.Id);
        }

        /// <summary>
        /// Returns mean degree
        /// </summary>
        public int GetDegreeMean()
        {
            int sum = Nodes.Sum(node => node.Degree);
            return sum / Nodes.Count;
        }

        /// <summary>
        /// Returns maximal closeness centrality
        /// </summary>
        public double GetClosenessCentralityMax()
        {
            return Nodes.Select(x => x.ClosenessCentrality).ToList().Max();
        }

        /// <summary>
        /// Get maximal mesiacy score
        /// </summary>
        public double GetMediacyScoreMax()
        {
            return Nodes.Select(x => x.MediacyScore).ToList().Max();
        }

        /// <summary>
        /// Gets incident communities for a node
        /// </summary>
        public HashSet<Community<T>> GetIncidentCommunitiesOfNode(Node<T> node)
        {
            HashSet<Community<T>> incidentCommunities = new HashSet<Community<T>> { node.Community };
            HashSet<Node<T>> adjacentNodes = GetAdjacentNodes(node);
            foreach (Node<T> adjacent in adjacentNodes)
            {
                Node<T> adjacentNode = GetNodeById(adjacent.Id);
                if (incidentCommunities.All(x => x.Id != adjacentNode.Community.Id))
                {
                    incidentCommunities.Add(adjacentNode.Community);
                }
            }
            return incidentCommunities;
        }

        public void SetCommunities(Dictionary<int, List<int>> dictCommunities)
        {
            foreach (KeyValuePair<int, List<int>> kvp in dictCommunities)
            {
                HashSet<Node<T>> communityNodes = new HashSet<Node<T>>();
                foreach (int i in kvp.Value)
                {
                    Node<T> node = GetNodeById(i);
                    communityNodes.Add(GetNodeById(i));
                    node.CommunityId = kvp.Key;
                }
                Community<T> community = new Community<T>(kvp.Key, communityNodes);
                foreach (Node<T> node in communityNodes)
                {
                    node.Community = community;
                }
                Communities.Add(community);
            }
        }
    }
}
