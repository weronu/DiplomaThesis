using System;
using System.Collections.Generic;
using System.Linq;
using Domain.DTOs;
using Domain.GraphClasses;
using MoreLinq;

namespace Graph.Algorithms
{
    public class EgoNetwork
    {
        /// <summary>
        /// Compute connected subgraphs of the graph
        /// </summary>
        /// <param name="graph"></param>
        /// <returns>Collection of collections with nodes, that form a connected subgraph</returns>
        public HashSet<HashSet<Node<UserDto>>> FindConectedSubgraphs(Graph<UserDto> graph)
        {
            HashSet<HashSet<Node<UserDto>>> subGraphs = new HashSet<HashSet<Node<UserDto>>>();
            foreach (Edge<UserDto> edge in graph.Edges)
            {
                if (!subGraphs.Any(x => x.Any(y => y.Id == edge.Node1.Id) || x.Any(y => y.Id == edge.Node2.Id)))
                {
                    subGraphs.Add(new HashSet<Node<UserDto>>() { edge.Node1, edge.Node2 }); //con.X is not part of any group, so we can create a new one with X and Y added, since both a are in the group       
                }
                else
                {
                    if (subGraphs.Count(x => x.Any(y => y.Id == edge.Node1.Id) || x.Any(y => y.Id == edge.Node2.Id)) == 1)
                    {
                        HashSet<Node<UserDto>> group = subGraphs.First(g => g.Any(x => x.Id == edge.Node1.Id) || g.Any(y => y.Id == edge.Node2.Id));
                        if (group.All(y => y.Id != edge.Node1.Id)) group.Add(edge.Node1);
                        if (group.All(y => y.Id != edge.Node2.Id)) group.Add(edge.Node2);
                    }
                    if (subGraphs.Count(x => x.Any(y => y.Id == edge.Node1.Id) || x.Any(y => y.Id == edge.Node2.Id)) > 1)
                    {
                        HashSet<Node<UserDto>> groupUnion = new HashSet<Node<UserDto>>();
                        foreach (HashSet<Node<UserDto>> grp in subGraphs.Where(g => g.Any(x => x.Id == edge.Node1.Id) || g.Any(y => y.Id == edge.Node2.Id)).ToList())
                        {
                            groupUnion = new HashSet<Node<UserDto>>(groupUnion.Union(grp));
                            subGraphs.Remove(grp);
                        }
                        if (groupUnion.All(x => x.Id != edge.Node1.Id)) groupUnion.Add(edge.Node1);
                        if (groupUnion.All(x => x.Id != edge.Node2.Id)) groupUnion.Add(edge.Node2);
                        subGraphs.Add(groupUnion);
                    }
                }
            }

            return subGraphs;
        }

        private static void WriteSubgraphsToConsole(IEnumerable<HashSet<Node<UserDto>>> subGraphs)
        {
            foreach (HashSet<Node<UserDto>> subGraph in subGraphs)
            {
                foreach (Node<UserDto> node in subGraph)
                {
                    Console.WriteLine(node.Id);
                }
                Console.WriteLine("-------------");
            }
        }

        public HashSet<Node<UserDto>> GetNodesWithMaximalDegreeInSubgraphs(HashSet<HashSet<Node<UserDto>>> subGraphs, Node<UserDto> EgoNetworkCenter)
        {
            try
            {
                HashSet<Node<UserDto>> nodes = new HashSet<Node<UserDto>>();
                foreach (HashSet<Node<UserDto>> subGraph in subGraphs)
                {
                    if (subGraph.All(x => x.Id != EgoNetworkCenter.Id))
                    {
                        Node<UserDto> nodeWithMaxDegreeInSubgraph = subGraph.Where(x => x.Id != EgoNetworkCenter.Id).MaxBy(i => i.Degree); // except the center of ego network
                        nodes.Add(nodeWithMaxDegreeInSubgraph);
                    }

                }
                return nodes;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public double GetEIIndex(Graph<UserDto> graph, Node<UserDto> egoNetworkCenter)
        {
            double index = 0;
            try
            {
                if (graph.Nodes.First().Community == null)
                {
                    return 0;
                }

                HashSet<Node<UserDto>> alters = graph.GetAdjacentNodes(egoNetworkCenter);

                int e = 0;
                int i = 0;

                foreach (Node<UserDto> alter in alters)
                {
                    if (egoNetworkCenter.CommunityId == alter.CommunityId)
                    {
                        i++;
                    }
                    else if(egoNetworkCenter.CommunityId != alter.CommunityId)
                    {
                        e++;
                    }
                }

                index = (double) (e - i) / (e + i);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return index;
        }

        public double GetEffectiveSizeOfEgo(Graph<UserDto> graph, Node<UserDto> egoNetworkCenter)
        {
            double effectiveSize = 0;
            try
            {
                HashSet<Node<UserDto>> alters = graph.GetAdjacentNodes(egoNetworkCenter);

                int numberOfAlters = alters.Count;
                int numberOfTies = 0;

                HashSet<Edge<UserDto>> adjacentEdgesOfEgo = graph.GetAdjacentEdges(egoNetworkCenter);

                List<Edge<UserDto>> tiesInEgoNetwork = new List<Edge<UserDto>>();
                foreach (Node<UserDto> alter in alters)
                {
                    HashSet<Edge<UserDto>> adjacentEdgesOfAlter = graph.GetAdjacentEdges(alter);
                    List<Edge<UserDto>> list = adjacentEdgesOfAlter.Where(x => alters.Any(i => i.Id == x.Node1.Id) && alters.Any(i => i.Id == x.Node2.Id)).ToList();

                    foreach (Edge<UserDto> edge in adjacentEdgesOfEgo)
                    {
                        list.Remove(edge);
                    }
                    tiesInEgoNetwork.AddRange(list);
                }

                tiesInEgoNetwork = tiesInEgoNetwork.Distinct().ToList();

                numberOfTies = tiesInEgoNetwork.Count;
                int pom = (2 * numberOfTies);

                effectiveSize = numberOfAlters - (double)((double)(pom / (double)numberOfAlters));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return Math.Round(effectiveSize);
        }

        public int GetNumberOfConnectedCommunities(Graph<UserDto> graph, Node<UserDto> egoCenter)
        {
            try
            {
                if (graph.Communities.Count == 0)
                {
                    throw new Exception("You have to detect communities first!");
                }
                HashSet<Node<UserDto>> alters = graph.GetAdjacentNodes(egoCenter);
                List<int> connnectedCommunities = alters.Select(x => x.CommunityId).Distinct().ToList();
                return connnectedCommunities.Count;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
