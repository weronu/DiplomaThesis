﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domain.DomainClasses;
using Domain.DTOs;
using Domain.GraphClasses;

namespace Graph.Algorithms
{
    public static class LouvainCommunity
    {
        private static readonly int PASS_MAX = -1;
        private static readonly double MIN = 0.0000001;

        /// <summary>
        /// Compute the partition of the graph nodes which maximises the modularity using the Louvain heuristics
        /// 
        /// This is the partition of the highest modularity, i.e., the highest partition of the dendrogram generated by the Louvain algorithm.
        /// 
        /// See also: GenerateDendrogram to obtain all the decomposition levels
        /// 
        /// Notes: Uses the Louvain algorithm
        /// 
        /// </summary>
        /// <param name="graph">The graph which is decomposed.</param>
        /// <param name="partition">The algorithm will start using this partition of nodes. It is a dictionary where keys are nodes and values are communities.</param>
        /// <returns>The partition, with communities number from 0 onward, sequentially</returns>
        public static Dictionary<int, int> BestPartition(Graph<UserDto> graph, Dictionary<int, int> partition)
        {
            Dendrogram dendro = GenerateDendrogram(graph, partition);
            Dictionary<int, int> partitionAtLevel = dendro.PartitionAtLevel(dendro.Length - 1);
            return partitionAtLevel;
        }

        /// <summary>
        /// Does BestPartition without an initial partition.
        /// </summary>
        /// <param name="graph">The graph for which to find the best partition.</param>
        /// <returns>The best partition of the graph.</returns>
        public static Dictionary<int, int> BestPartition(Graph<UserDto> graph)
        {
            return BestPartition(graph, null);
        }

        public static Dendrogram GenerateDendrogram(Graph<UserDto> graph, Dictionary<int, int> part_init)
        {
            Dictionary<int, int> partition;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();

            // Special case, when there is no link, the best partition is everyone in its own community.
            if (graph.Edges.Count == 0)
            {
                partition = new Dictionary<int, int>();
                int i = 0;
                foreach (Node<UserDto> node in graph.Nodes)
                {
                    partition[node.Id] = i++;
                }
                return new Dendrogram(partition);
            }

            //Graph current_graph = new Graph(graph);
            Graph<UserDto> current_graph = graph;
            Status status = new Status(current_graph, part_init);
            double mod = status.Modularity();
            List<Dictionary<int, int>> status_list = new List<Dictionary<int, int>>();
            status.OneLevel(current_graph);
            double new_mod = status.Modularity();

            int iterations = 1;
            do
            {
                iterations++;
                partition = Renumber(status.Node2Community);
                status_list.Add(partition);
                mod = new_mod;
                current_graph = current_graph.Quotient(partition);
                status = new Status(current_graph, null);
                status.OneLevel(current_graph);
                new_mod = status.Modularity();
            } while (new_mod - mod >= MIN);
            //Console.Out.WriteLine("(GenerateDendrogram: {0} iterations in {1})", iterations, stopwatch.Elapsed);

            return new Dendrogram(status_list);
        }

        private static Dictionary<A, int> Renumber<A>(Dictionary<A, int> dict)
        {
            Dictionary<A, int> ret = new Dictionary<A, int>();
            Dictionary<int, int> new_values = new Dictionary<int, int>();

            foreach (A key in dict.Keys.OrderBy(a => a))
            {
                int value = dict[key];
                int new_value;
                if (!new_values.TryGetValue(value, out new_value))
                {
                    new_value = new_values[value] = new_values.Count;
                }
                ret[key] = new_value;
            }
            return ret;
        }
        private static B DictGet<A, B>(Dictionary<A, B> dict, A key, B defaultValue)
        {
            B result;
            return dict.TryGetValue(key, out result) ? result : defaultValue;
        }

        /// <summary>
        /// To handle several pieces of data for the algorithm in one structure.
        /// </summary>
        public class Status
        {
            public Dictionary<int, int> Node2Community;
            public double TotalWeight;
            public Dictionary<int, double> Degrees;
            public Dictionary<int, double> GDegrees;
            public Dictionary<int, double> Loops;
            public Dictionary<int, double> Internals;

            public Status()
            {
                Node2Community = new Dictionary<int, int>();
                TotalWeight = 0;
                Degrees = new Dictionary<int, double>();
                GDegrees = new Dictionary<int, double>();
                Loops = new Dictionary<int, double>();
                Internals = new Dictionary<int, double>();
            }

            public Status(Graph<UserDto> graph, Dictionary<int, int> part): this()
            {
                int count = 0;
                TotalWeight = graph.Size;
                if (part == null)
                {
                    List<Node<UserDto>> vertices = graph.Nodes.OrderByDescending(x => x.Id).ToList();
                    foreach (Node<UserDto> node in vertices)
                    {
                        Node2Community[node.Id] = count;
                        double deg = graph.LouvainDegree(node);
                        if (deg < 0)
                        {
                            throw new ArgumentException("Graph has negative weights.");
                        }
                        Degrees[count] = GDegrees[node.Id] = deg;
                        Internals[count] = Loops[node.Id] = graph.GetEdgeWeight(node, node);
                        count += 1;
                    }
                }
                else
                {
                    foreach (Node<UserDto> node in graph.Nodes)
                    {
                        int com = part[node.Id];
                        Node2Community[node.Id] = com;
                        double deg = graph.GetNodeDegreeAsSumOfWeights(node);
                        Degrees[com] = DictGet(Degrees, com, 0) + deg;
                        GDegrees[node.Id] = deg;
                        double inc = 0;
                        foreach (Edge<UserDto> edge in graph.GetIncidentEdges(node))
                        {
                            int neighbor = edge.Node2.Id;
                            if (edge.Weight <= 0)
                            {
                                throw new ArgumentException("Graph must have postive weights.");
                            }
                            if (part[neighbor] == com)
                            {
                                if (neighbor == node.Id)
                                {
                                    inc += edge.Weight;
                                }
                                else
                                {
                                    inc += edge.Weight / 2;
                                }
                            }
                        }
                        Internals[com] = DictGet(Internals, com, 0) + inc;
                    }
                }
            }

            /// <summary>
            /// Compute the modularity of the partition of the graph fast using precomputed status.
            /// </summary>
            /// <returns></returns>
            public double Modularity()
            {
                double links = TotalWeight;
                double result = 0;
                foreach (int community in Node2Community.Values.Distinct())
                {
                    double in_degree = DictGet(Internals, community, 0);
                    double degree = DictGet(Degrees, community, 0);
                    if (links > 0)
                    {
                        result += in_degree / links - Math.Pow(degree / (2 * links), 2);
                    }
                }
                return result;
            }

            /// <summary>
            /// Used in parallelized OneLevel
            /// </summary>
            private Tuple<double, int> EvaluateIncrease(int com, double dnc, double degc_totw)
            {
                double incr = dnc - DictGet(Degrees, com, 0) * degc_totw;
                return Tuple.Create(incr, com);
            }

            /// <summary>
            /// Compute one level of communities.
            /// </summary>
            /// <param name="graph">The graph to use.</param>
            public void OneLevel(Graph<UserDto> graph)
            {
                bool modif = true;
                int nb_pass_done = 0;
                double cur_mod = this.Modularity();
                double new_mod = cur_mod;

                while (modif && nb_pass_done != PASS_MAX)
                {
                    cur_mod = new_mod;
                    modif = false;
                    nb_pass_done += 1;

                    List<Node<UserDto>> vertices = graph.Nodes.OrderByDescending(x => x.Id).ToList();
                    foreach (Node<UserDto> node in vertices)
                    {
                        int com_node = Node2Community[node.Id];
                        double degc_totw = DictGet<int, double>(GDegrees, node.Id, 0) / (TotalWeight * 2);
                        Dictionary<int, double> neigh_communities = NeighCom(node, graph);
                        Remove(node.Id, com_node, DictGet(neigh_communities, com_node, 0));

                        Tuple<double, int> best = (from entry in neigh_communities.AsParallel()
                                select EvaluateIncrease(entry.Key, entry.Value, degc_totw))
                            .Concat(new[] {Tuple.Create(0.0, com_node)}.AsParallel())
                            .Max();
                        int best_com = best.Item2;
                        Insert(node.Id, best_com, DictGet(neigh_communities, best_com, 0));
                        if (best_com != com_node)
                        {
                            modif = true;
                        }
                    }
                    new_mod = this.Modularity();
                    if (new_mod - cur_mod < MIN)
                    {
                        break;
                    }
                }
            }


            /// <summary>
            /// Compute the communities in th eneighborhood of the node in the given graph.
            /// </summary>
            /// <param name="node"></param>
            /// <param name="graph"></param>
            /// <returns></returns>
            private Dictionary<int, double> NeighCom(Node<UserDto> node, Graph<UserDto> graph)
            {
                Dictionary<int, double> weights = new Dictionary<int, double>();
                List<Edge<UserDto>> incidentEdgesLouvain = graph.GetIncidentEdges(node).ToList();

                foreach (Edge<UserDto> edge in incidentEdgesLouvain)
                {
                    if (!edge.SelfLoop)
                    {
                        int neighborcom = Node2Community[edge.Node2.Id];
                        weights[neighborcom] = DictGet(weights, neighborcom, 0) + edge.Weight;
                    }
                }
                return weights;
            }


            /// <summary>
            /// Remove node from community com and modify status.
            /// </summary>
            /// <param name="node"></param>
            /// <param name="com"></param>
            /// <param name="weight"></param>
            private void Remove(int node, int com, double weight)
            {
                Degrees[com] = DictGet(Degrees, com, 0) - DictGet(GDegrees, node, 0);
                Internals[com] = DictGet(Internals, com, 0) - weight - DictGet(Loops, node, 0);
                Node2Community[node] = -1;
            }

            /// <summary>
            /// Insert node into community and modify status.
            /// </summary>
            /// <param name="node"></param>
            /// <param name="com"></param>
            /// <param name="weight"></param>
            private void Insert(int node, int com, double weight)
            {
                Node2Community[node] = com;
                Degrees[com] = DictGet(Degrees, com, 0) + DictGet(GDegrees, node, 0);
                Internals[com] = DictGet(Internals, com, 0) + weight + DictGet(Loops, node, 0);
            }
        }
    }

    /// <summary>
    /// A dendrogram is a tree, and each level is a partition of the graph nodes. Level 0 is the first partition, which contains the smallest communities,
    /// and the largest (best) are in dendrogram.Length - 1.
    /// </summary>
    public class Dendrogram
    {
        private readonly List<Dictionary<int, int>> Partitions;

        /// <summary>
        /// Creates a dendrogram with one level.
        /// </summary>
        /// <param name="part">The partition for the one level.</param>
        public Dendrogram(Dictionary<int, int> part)
        {
            Partitions = new List<Dictionary<int, int>> {part};
        }

        /// <summary>
        /// Creates a dendrogram with multiple levels.
        /// </summary>
        /// <param name="parts"></param>
        public Dendrogram(IEnumerable<Dictionary<int, int>> parts)
        {
            Partitions = new List<Dictionary<int, int>>(parts);
        }

        public int Length => Partitions.Count;

        /// <summary>
        /// Return the partition of the nodes at the given level.
        /// </summary>
        /// <param name="level">The level to retrieve, [0..dendrogram.Length-1].</param>
        /// <returns>A dictionary where keys are nodes and values the set to which it belongs.</returns>
        public Dictionary<int, int> PartitionAtLevel(int level)
        {
            Dictionary<int, int> partition = new Dictionary<int, int>(Partitions[0]);
            for (int index = 1; index <= level; index++)
            {
                foreach (int node in partition.Keys.ToArray())
                {
                    int com = partition[node];
                    partition[node] = Partitions[index][com];
                }
            }
            return partition;
        }
    }
}