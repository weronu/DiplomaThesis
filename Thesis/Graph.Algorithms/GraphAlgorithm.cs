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
        public HashSet<ShortestPathSet<T>> GetAllShortestPathsInGraph(HashSet<Node<T>> nodes)
        {
            HashSet<ShortestPathSet<T>> shortestPaths = new HashSet<ShortestPathSet<T>>();
            for (int i = 0; i < nodes.Count; i++)
            {
                Node<T> nodei = nodes.ElementAt(i);
                for (int j = i + 1; j < nodes.Count; j++)
                {
                    Node<T> nodej = nodes.ElementAt(j);
                    {
                        Func<Node<T>, HashSet<Node<T>>> shortestPath = ShortestPathFunction(nodei);
                        HashSet<Node<T>> path = shortestPath(nodej);
                        shortestPaths.Add(new ShortestPathSet<T>() { StartNode = nodei, EndNode = nodej, ShortestPath = path });
                        IEnumerable<Node<T>> reversePath = path.Reverse();
                        shortestPaths.Add(new ShortestPathSet<T>() { StartNode = nodej, EndNode = nodei, ShortestPath = new HashSet<Node<T>>(reversePath) });
                    }
                }
            }
            return shortestPaths;
        }


        /// <summary>
        /// Computes closeness centrality for a node
        /// </summary>
        public double GetClosenessCentrality(Node<T> startNode, HashSet<ShortestPathSet<T>> shortestPaths)
        {
            int n = _graph.Nodes.Count;
            int sum = shortestPaths.Where(x => (x.StartNode.Id != x.EndNode.Id) && (x.StartNode.Id == startNode.Id)).Select(x => x.ShortestPath.Count - 1).Sum();

            return Math.Round((double)n / sum, 2);
        }

        /// <summary>
        /// Computes closeness centrality for node in its community
        /// </summary>
        public double GetClosenessCentralityInCommunity(Node<T> startNode, HashSet<ShortestPathSet<T>> shortestPaths)
        {
            if (_graph.Communities == null) return 0;

            //get nodes community
            Community<T> community = _graph.GetCommunityById(startNode.CommunityId);
            int n = community.CommunityNodes.Count; //count of nodes in community
            int sum = shortestPaths.Where(x => x.StartNode == startNode && x.EndNode.CommunityId == startNode.CommunityId).Select(x => x.ShortestPath.Count - 1).Sum();

            return Math.Round((double)n / sum, 2);
        }

        /// <summary>
        /// Shortest path function
        /// </summary>
        public Func<Node<T>, HashSet<Node<T>>> ShortestPathFunction(Node<T> start)
        {
            Dictionary<Node<T>, Node<T>> previous = new Dictionary<Node<T>, Node<T>>();

            Queue<Node<T>> queue = new Queue<Node<T>>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                Node<T> node = queue.Dequeue();
                HashSet<Node<T>> adjacentNodes = _graph.GetAdjacentNodes(node);
                foreach (Node<T> neighbor in adjacentNodes)
                {
                    if (previous.Keys.Any(x => x.Id == neighbor.Id))
                    {
                        continue;
                    }

                    previous[neighbor] = node;
                    queue.Enqueue(neighbor);
                }
            }

            Func<Node<T>, HashSet<Node<T>>> shortestPath = v =>
            {
                HashSet<Node<T>> path = new HashSet<Node<T>>();

                Node<T> current = v;

                while ((current != null) && !current.Equals(start))
                {
                    path.Add(current);

                    Node<T> current1 = current;
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
            List<double> values = community.CommunityNodes.Select(x => x.ClosenessCentralityInCommunity).ToList<double>();

            double sd = 0;
            if (values.Any())
            {
                double avg = values.Average();
                double sumOfSquaresOfDifferences = values.Select(val => (val - avg) * (val - avg)).Sum();
                sd = Math.Sqrt(sumOfSquaresOfDifferences / values.Count);
            }
            return sd;
        }

        /// <summary>
        /// Computes mean of closeness centrality in community
        /// </summary>
        public double GetCommunityClosenessCentralityMean(Community<T> community)
        {
            return community.CommunityNodes.Select(x => x.ClosenessCentralityInCommunity).Average();
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
        /// Computes CBetweeness for a node in a graph
        /// </summary>
        public void CBetweeness(Node<T> node, HashSet<ShortestPathSet<T>> cPath)
        {
            int i = 0;
            if (cPath.Any(x => x.ShortestPath.Any(y => y.Id == node.Id)))
            {
                i += 1;
            }
            node.CBetweeness = i / 2;
        }

        /// <summary>
        /// Computes normalized CBetweeness for a node in a graph
        /// </summary>
        public double NormalizedCBC(Node<T> node, HashSet<ShortestPathSet<T>> cPath)
        {
            double sum = 0.0;

            foreach (ShortestPathSet<T> pathSet in cPath)
            {
                Community<T> csp = _graph.GetCommunityById(pathSet.StartNode.CommunityId);
                Community<T> cep = _graph.GetCommunityById(pathSet.EndNode.CommunityId);
                int ip = 0;

                if (CBCId(pathSet, node))
                {
                    ip = 1;
                }
                int min = Math.Min(csp.CommunityNodes.Count, cep.CommunityNodes.Count);
                double frac = (double)ip / min;
                sum += frac;

            }
            return (double)sum / 2;
        }

        private bool CBCId(ShortestPathSet<T> pathSet, Node<T> node)
        {
            return pathSet.ShortestPath.Any(x => x.Id == node.Id);
        }

        /// <summary>
        /// Computes number of distinct communities passing thourgh node 
        /// </summary>
        public double DSCount(Node<T> node, HashSet<ShortestPathSet<T>> cPath)
        {
            int i = _graph.Communities.Count(community => DsCountId(cPath, community, node));
            return (double) i / 2;
        }

        private bool DsCountId(IEnumerable<ShortestPathSet<T>> cPath, Community<T> community, Node<T> node)
        {
            bool res = cPath.Any(pathSet => community.CommunityNodes.Any(x => x.Id == pathSet.StartNode.Id) && (pathSet.ShortestPath.Any(x => x.Id == node.Id)));
            return res;
        }

        /// <summary>
        /// Ordering nodes by their mediacy score
        /// </summary>
        public HashSet<Node<T>> OrderNodesByMediacyScore()
        {
            return new HashSet<Node<T>>((_graph.Nodes.OrderByDescending(x => x.MediacyScore).ToList()));
        }


        /// <summary>
        /// Set normalized CBetweeness for each node in its community
        /// </summary>
        public void SetDSCountForEachNode(HashSet<ShortestPathSet<T>> cPaths)
        {
            foreach (Node<T> node in _graph.Nodes)
            {
                node.DSCount = DSCount(node, cPaths);
            }
        }


        /// <summary>
        /// Sets closeness centrality for each node in a graph
        /// </summary>
        public void SetClosenessCentralityForEachNode(HashSet<ShortestPathSet<T>> allShortestPathSets)
        {
            foreach (Node<T> node in _graph.Nodes)
            {
                node.ClosenessCentrality = GetClosenessCentrality(node, allShortestPathSets);
            }
        }

        /// <summary>
        /// Set closeness centrality for each node in its community
        /// </summary>
        public void SetClosenessCentralityForEachNodeInCommunity(HashSet<ShortestPathSet<T>> allShortestPathSets)
        {
            foreach (Node<T> node in _graph.Nodes)
            {
                node.ClosenessCentralityInCommunity = GetClosenessCentralityInCommunity(node, allShortestPathSets);
            }
        }

        /// <summary>
        /// Set normalized CBetweeness for each node in its community
        /// </summary>
        public void SetNCBCForEachNode(HashSet<ShortestPathSet<T>> cPaths)
        {
            foreach (Node<T> node in _graph.Nodes)
            {
                node.NormalizedCBC = NormalizedCBC(node, cPaths);
            }
        }

        /// <summary>
        /// Mean for closeness centrality in each community
        /// </summary>
        public void SetMeanClosenessCentralityForEachCommunity()
        {
            foreach (Community<T> community in _graph.Communities)
            {
                community.ClosenessCentralityMean = GetCommunityClosenessCentralityMean(community);
            }
        }

        /// <summary>
        /// Standart deviation for closeness centrality in each community
        /// </summary>
        public void SetStandartDeviationForClosenessCentralityForEachCommunity()
        {
            foreach (Community<T> community in _graph.Communities)
            {
                community.ClosenessCentralityStandartDeviation = GetCommunityClosenessCentralityStandartDeviation(community);
            }
        }
    }
}
