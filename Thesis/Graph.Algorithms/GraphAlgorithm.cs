using System;
using System.Collections.Generic;
using System.Linq;
using Domain.DomainClasses;
using Domain.Enums;
using Domain.GraphClasses;

namespace Graph.Algorithms
{
    public class GraphAlgorithm<T> where T : DomainBase
    {
        private Graph<T> _graph;

        public GraphAlgorithm(Graph<T> graph)
        {
            this._graph = graph;
        }

        /// <summary>
        /// Returns all shortest paths in a graph
        /// </summary>
        public HashSet<ShortestPathSet<T>> GetAllShortestPathsInGraph(HashSet<Vertex<T>> vertices)
        {
            HashSet<ShortestPathSet<T>> shortestPaths = new HashSet<ShortestPathSet<T>>();
            for (int i = 0; i < vertices.Count; i++)
            {
                Vertex<T> vertexi = vertices.ElementAt(i);
                for (int j = i + 1; j < vertices.Count; j++)
                {
                    Vertex<T> vertexj = vertices.ElementAt(j);
                    {
                        Func<Vertex<T>, HashSet<Vertex<T>>> shortestPath = ShortestPathFunction(vertexi);
                        HashSet<Vertex<T>> path = shortestPath(vertexj);
                        shortestPaths.Add(new ShortestPathSet<T>() { StartNode = vertexi, EndNode = vertexj, ShortestPath = path });
                        IEnumerable<Vertex<T>> reversePath = path.Reverse();
                        shortestPaths.Add(new ShortestPathSet<T>() { StartNode = vertexj, EndNode = vertexi, ShortestPath = new HashSet<Vertex<T>>(reversePath) });
                    }
                }
            }
            return shortestPaths;
        }


        /// <summary>
        /// Computes closeness centrality for a vertex
        /// </summary>
        public double GetClosenessCentrality(Vertex<T> startNode, HashSet<ShortestPathSet<T>> shortestPaths)
        {
            var n = _graph.Vertices.Count;
            var sum = shortestPaths.Where(x => (x.StartNode.Id != x.EndNode.Id) && (x.StartNode.Id == startNode.Id)).Select(x => x.ShortestPath.Count - 1).Sum();

            return Math.Round((double)n / sum, 2);
        }

        /// <summary>
        /// Computes closeness centrality for vertex in its community
        /// </summary>
        public double GetClosenessCentralityInCommunity(Vertex<T> startNode, HashSet<ShortestPathSet<T>> shortestPaths)
        {
            if (_graph.Communities == null) return 0;

            //get vertices community
            Community<T> community = _graph.GetCommunityById(startNode.CommunityId);
            int n = community.CommunityVertices.Count; //count of vertices in community
            var sum = shortestPaths.Where(x => x.StartNode == startNode && x.EndNode.CommunityId == startNode.CommunityId).Select(x => x.ShortestPath.Count - 1).Sum();

            return Math.Round((double)n / sum, 2);
        }

        /// <summary>
        /// Shortest path function
        /// </summary>
        public Func<Vertex<T>, HashSet<Vertex<T>>> ShortestPathFunction(Vertex<T> start)
        {
            Dictionary<Vertex<T>, Vertex<T>> previous = new Dictionary<Vertex<T>, Vertex<T>>();

            Queue<Vertex<T>> queue = new Queue<Vertex<T>>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                Vertex<T> vertex = queue.Dequeue();
                HashSet<Vertex<T>> adjacentNodes = _graph.GetAdjacentVertices(vertex);
                foreach (Vertex<T> neighbor in adjacentNodes)
                {
                    if (previous.Keys.Any(x => x.Id == neighbor.Id))
                    {
                        continue;
                    }

                    previous[neighbor] = vertex;
                    queue.Enqueue(neighbor);
                }
            }

            Func<Vertex<T>, HashSet<Vertex<T>>> shortestPath = v =>
            {
                HashSet<Vertex<T>> path = new HashSet<Vertex<T>>();

                Vertex<T> current = v;

                while ((current != null) && !current.Equals(start))
                {
                    path.Add(current);

                    Vertex<T> current1 = current;
                    current = previous.Where(pair => pair.Key.Id == current1.Id)
                        .Select(pair => pair.Value)
                        .FirstOrDefault();
                };

                path.Add(start);
                path.Reverse();

                return path;
            };

            return shortestPath;
        }

        /// <summary>
        /// Computes standart deviation of closeness centrality in community
        /// </summary>
        public double GetCommunityClosenessCentralityStandartDeviation(Community<T> community)
        {
            List<double> values = community.CommunityVertices.Select(x => x.ClosenessCentralityInCommunity).ToList<double>();

            double sd = 0;
            if (values.Any())
            {
                var avg = values.Average();
                var sumOfSquaresOfDifferences = values.Select(val => (val - avg) * (val - avg)).Sum();
                sd = Math.Sqrt(sumOfSquaresOfDifferences / values.Count);
            }
            return sd;
        }

        /// <summary>
        /// Computes mean of closeness centrality in community
        /// </summary>
        public double GetCommunityClosenessCentralityMean(Community<T> community)
        {
            return community.CommunityVertices.Select(x => x.ClosenessCentralityInCommunity).Average();
        }
       
        /// <summary>
        /// Get list of all LPaths in a graph
        /// </summary>
        public HashSet<ShortestPathSet<T>> LPaths(HashSet<ShortestPathSet<T>> allShortestPaths)
        {
            HashSet<ShortestPathSet<T>> lPaths = new HashSet<ShortestPathSet<T>>(allShortestPaths.Where(x => x.StartNode.Role == Role.Leader || x.EndNode.Role == Role.Leader && x.StartNode.CommunityId != x.EndNode.CommunityId));

            return lPaths;
        }

        /// <summary>
        /// Get list of all CPaths in a graph
        /// </summary>
        public HashSet<ShortestPathSet<T>> CPaths(HashSet<ShortestPathSet<T>> allShortestPaths)
        {
            IEnumerable<ShortestPathSet<T>> cPathsIEnumerable = allShortestPaths.Where(x => x.StartNode.CommunityId != x.EndNode.CommunityId && x.StartNode.Id != x.EndNode.Id);
            HashSet<ShortestPathSet<T>> cPaths = new HashSet<ShortestPathSet<T>>(cPathsIEnumerable);

            return cPaths;
        }

        /// <summary>
        /// Computes CBetweeness for a vertex in a graph
        /// </summary>
        public void CBetweeness(Vertex<T> vertex, HashSet<ShortestPathSet<T>> cPath)
        {
            var i = 0;

            if (cPath.Any(x => x.ShortestPath.Contains(vertex)))
            {
                i += 1;
            }
            vertex.CBetweeness = i / 2;
        }

        /// <summary>
        /// Computes normalized CBetweeness for a vertex in a graph
        /// </summary>
        public double NormalizedCBC(Vertex<T> vertex, HashSet<ShortestPathSet<T>> cPath)
        {
            var sum = 0.0;

            foreach (ShortestPathSet<T> pathSet in cPath)
            {
                Community<T> csp = _graph.GetCommunityById(pathSet.StartNode.CommunityId);
                Community<T> cep = _graph.GetCommunityById(pathSet.EndNode.CommunityId);
                var ip = 0;

                if (CBCId(pathSet, vertex))
                {
                    ip = 1;
                }
                var min = Math.Min(csp.CommunityVertices.Count, cep.CommunityVertices.Count);
                double frac = (double)ip / min;
                sum += frac;

            }
            return (double)sum / 2;
        }

        private bool CBCId(ShortestPathSet<T> pathSet, Vertex<T> vertex)
        {
            return pathSet.ShortestPath.Contains(vertex);
        }

        /// <summary>
        /// Computes number of distinct communities passing thourgh vertex 
        /// </summary>
        public double DSCount(Vertex<T> vertex, HashSet<ShortestPathSet<T>> cPath)
        {
            int i = 0;
            foreach (Community<T> community in _graph.Communities)
            {
                if (DsCountId(cPath, community, vertex))
                {
                    i += 1;
                }
            }
            return (double) i / 2;
        }

        private bool DsCountId(HashSet<ShortestPathSet<T>> cPath, Community<T> community, Vertex<T> vertex)
        {
            bool res = false;
            foreach (ShortestPathSet<T> pathSet in cPath)
            {
                if (community.CommunityVertices.Contains(pathSet.StartNode) && (pathSet.ShortestPath.Contains(vertex)))
                {
                    return true;
                }

            }
            return res;
            //return cPath.Any(x => x.StartNode.CommunityId == community.Id && x.ShortestPath.Contains(vertex));
        }

        /// <summary>
        /// Ordering vertices by their mediacy score
        /// </summary>
        public List<Vertex<T>> OrderNodesByMediacyScore()
        {
            return _graph.Vertices.OrderByDescending(x => x.MediacyScore).ToList();
        }


        /// <summary>
        /// Set normalized CBetweeness for each vertex in its community
        /// </summary>
        public void SetDSCountForEachNode(HashSet<ShortestPathSet<T>> cPaths)
        {
            foreach (Vertex<T> vertex in _graph.Vertices)
                vertex.DSCount = DSCount(vertex, cPaths);
        }


        /// <summary>
        /// Sets closeness centrality for each vertex in a graph
        /// </summary>
        public void SetClosenessCentralityForEachNode(HashSet<ShortestPathSet<T>> allShortestPathSets)
        {
            foreach (Vertex<T> vertex in _graph.Vertices)
            {
                vertex.ClosenessCentrality = GetClosenessCentrality(vertex, allShortestPathSets);
            }
        }

        /// <summary>
        /// Set closeness centrality for each vertex in its community
        /// </summary>
        public void SetClosenessCentralityForEachNodeInCommunity(HashSet<ShortestPathSet<T>> allShortestPathSets)
        {
            foreach (Vertex<T> vertex in _graph.Vertices)
                vertex.ClosenessCentralityInCommunity = GetClosenessCentralityInCommunity(vertex, allShortestPathSets);
        }

        /// <summary>
        /// Set normalized CBetweeness for each vertex in its community
        /// </summary>
        public void SetNCBCForEachNode(HashSet<ShortestPathSet<T>> cPaths)
        {
            foreach (Vertex<T> vertex in _graph.Vertices)
                vertex.NormalizedCBC = NormalizedCBC(vertex, cPaths);
        }

        /// <summary>
        /// Mean for closeness centrality in each community
        /// </summary>
        public void SetMeanClosenessCentralityForEachCommunity()
        {
            foreach (Community<T> community in _graph.Communities)
                community.ClosenessCentralityMean = GetCommunityClosenessCentralityMean(community);
        }

        /// <summary>
        /// Standart deviation for closeness centrality in each community
        /// </summary>
        public void SetStandartDeviationForClosenessCentralityForEachCommunity()
        {
            foreach (Community<T> community in _graph.Communities)
                community.ClosenessCentralityStandartDeviation = GetCommunityClosenessCentralityStandartDeviation(community);
        }
    }
}
