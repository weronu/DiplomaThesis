using System;
using System.Collections.Generic;
using System.Linq;
using Domain.DomainClasses;
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
        public HashSet<HashSet<Node<User>>> FindConectedSubgraphs(Graph<User> graph)
        {
            HashSet<HashSet<Node<User>>> subGraphs = new HashSet<HashSet<Node<User>>>();
            foreach (Edge<User> edge in graph.Edges)
            {
                if (!subGraphs.Any(x => x.Any(y => y.Id == edge.Node1.Id) || x.Any(y => y.Id == edge.Node2.Id)))
                {
                    subGraphs.Add(new HashSet<Node<User>>() { edge.Node1, edge.Node2 }); //con.X is not part of any group, so we can create a new one with X and Y added, since both a are in the group       
                }
                else
                {
                    if (subGraphs.Count(x => x.Any(y => y.Id == edge.Node1.Id) || x.Any(y => y.Id == edge.Node2.Id)) == 1)
                    {
                        HashSet<Node<User>> group = subGraphs.First(g => g.Any(x => x.Id == edge.Node1.Id) || g.Any(y => y.Id == edge.Node2.Id));
                        if (group.All(y => y.Id != edge.Node1.Id)) group.Add(edge.Node1);
                        if (group.All(y => y.Id != edge.Node2.Id)) group.Add(edge.Node2);
                    }
                    if (subGraphs.Count(x => x.Any(y => y.Id == edge.Node1.Id) || x.Any(y => y.Id == edge.Node2.Id)) > 1)
                    {
                        HashSet<Node<User>> groupUnion = new HashSet<Node<User>>();
                        foreach (HashSet<Node<User>> grp in subGraphs.Where(g => g.Any(x => x.Id == edge.Node1.Id) || g.Any(y => y.Id == edge.Node2.Id)).ToList())
                        {
                            groupUnion = new HashSet<Node<User>>(groupUnion.Union(grp));
                            subGraphs.Remove(grp);
                        }
                        if (groupUnion.All(x => x.Id != edge.Node1.Id)) groupUnion.Add(edge.Node1);
                        if (groupUnion.All(x => x.Id != edge.Node2.Id)) groupUnion.Add(edge.Node2);
                        subGraphs.Add(groupUnion);
                    }
                }
            }

            return subGraphs;
            //foreach (HashSet<Node<User>> subGraph in subGraphs)
            //{
            //    foreach (Node<User> node in subGraph)
            //    {
            //        Console.WriteLine(node.Id);
            //    }
            //    Console.WriteLine("-------------");
            //}
        }

        public HashSet<Node<User>> GetNodesWithMaximalDegreeInSubgraphs(HashSet<HashSet<Node<User>>> subGraphs, Node<User> EgoNetworkCenter)
        {
            HashSet<Node<User>> nodes = new HashSet<Node<User>>();
            foreach (HashSet<Node<User>> subGraph in subGraphs)
            {
                if (subGraph.All(x => x.Id != EgoNetworkCenter.Id))
                {
                    Node<User> nodeWithMaxDegreeInSubgraph = subGraph.Where(x => x.Id != EgoNetworkCenter.Id).MaxBy(i => i.Degree); // except the center of ego network
                    nodes.Add(nodeWithMaxDegreeInSubgraph);
                }
               
            }
            return nodes;
        }
    }
}
