using System.Collections.Generic;
using System.Linq;
using Domain.DomainClasses;
using Domain.Enums;
using Domain.GraphClasses;

namespace Graph.Algorithms
{
    public class GraphRoleDetection<T> where T : DomainBase
    {
        private readonly Graph<T> _graph;
        private readonly GraphAlgorithm<T> _algorithms;

        public GraphRoleDetection(Graph<T> graph, GraphAlgorithm<T> algorithms)
        {
            _graph = graph;
            _algorithms = algorithms;
        }

        /// <summary>
        /// Extracts outsiders from a graph
        /// </summary>
        public void ExtractOutsiders()
        {
            foreach (Node<T> node in _graph.Nodes)
            {
                if (node.Community == null)
                {
                    node.Role = Role.Outsider;
                }
            }
        }

        /// <summary>
        /// Extracts leaders from a graph
        /// </summary>
        public void ExtractLeaders()
        {
            foreach (Community<T> community in _graph.Communities)
            {
                double meanClosenessCentralityMeanInCommunity = _algorithms.GetCommunityClosenessCentralityMean(community);
                double standartDeviationClosenessCentrality = _algorithms.GetCommunityClosenessCentralityStandartDeviation(community);
                double thresholdForLeaders = meanClosenessCentralityMeanInCommunity + (2 * standartDeviationClosenessCentrality);

                foreach (Node<T> node in community.CommunityNodes)
                {
                    if (node.ClosenessCentralityInCommunity > thresholdForLeaders)
                        node.Role = Role.Leader;
                }
            }
        }

        /// <summary>
        /// Extracts mediators from a graph
        /// </summary>
        public void ExtractMediators(HashSet<Node<T>> orderedNodes)
        {
            HashSet<Node<T>> mediatorSet = new HashSet<Node<T>>();
            HashSet<Community<T>> connectedCommunities = new HashSet<Community<T>>();
            while (connectedCommunities.Count < _graph.Communities.Count && orderedNodes.Count != 0)
            {
                Node<T> node = orderedNodes.First();
                foreach (Community<T> community in _graph.GetIncidentCommunitiesOfNode(node))
                {
                    if (connectedCommunities.All(x => x.Id != community.Id))
                    {
                        if (mediatorSet.All(x => x.Id != node.Id))
                        {
                            mediatorSet.Add(node);
                        }
                        connectedCommunities.Add(community);
                    }
                }
                orderedNodes.Remove(node);
            }

            foreach (Node<T> node in mediatorSet)
            {
                if (node.Role == 0)
                {
                    node.Role = Role.Mediator;
                }
            }
        }

        /// <summary>
        /// Extracts outermosts from a graph
        /// </summary>
        public void ExtractOutermosts()
        {
            foreach (Community<T> community in _graph.Communities)
            {
                double meanClosenessCentralityMeanInCommunity = _algorithms.GetCommunityClosenessCentralityMean(community);
                double standartDeviationClosenessCentrality = _algorithms.GetCommunityClosenessCentralityStandartDeviation(community);
                double thresholdForOutermosts = meanClosenessCentralityMeanInCommunity - (2 * standartDeviationClosenessCentrality); //mean - 2*standart deviation

                foreach (Node<T> node in community.CommunityNodes)
                {
                    if (node.ClosenessCentralityInCommunity < thresholdForOutermosts)
                    {
                        node.Role = Role.Outermost;
                    }
                }
            }
        }
    }
}
