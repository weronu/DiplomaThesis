using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.Enums;
using Domain.GraphClasses;

namespace Graph.Algorithms
{
    public class GraphRoleDetection<T> where T : DomainBase
    {
        private Graph<T> _graph;
        private GraphAlgorithm<T> _algorithms;

        public GraphRoleDetection(Graph<T> graph, GraphAlgorithm<T> algorithms)
        {
            this._graph = graph;
            this._algorithms = algorithms;
        }

        /// <summary>
        /// Extracts outsiders from a graph
        /// </summary>
        public void ExtractOutsiders()
        {
            foreach (var node in _graph.Vertices)
                if (node.Community == null)
                    node.Role = Role.Outsider;
        }

        /// <summary>
        /// Extracts leaders from a graph
        /// </summary>
        public void ExtractLeaders()
        {
            foreach (var community in _graph.Communities)
            {
                var meanClosenessCentralityMeanInCommunity = _algorithms.GetCommunityClosenessCentralityMean(community);
                var standartDeviationClosenessCentrality = _algorithms.GetCommunityClosenessCentralityStandartDeviation(community);
                var thresholdForLeaders = meanClosenessCentralityMeanInCommunity + (2 * standartDeviationClosenessCentrality); //mean + 2*standart deviation

                foreach (var node in community.CommunityVertices)
                    if (node.ClosenessCentralityInCommunity > thresholdForLeaders)
                        node.Role = Role.Leader;
            }
        }

        /// <summary>
        /// Extracts mediators from a graph
        /// </summary>
        public void ExtractMediators(List<Vertex<T>> orderedNodes)
        {
            var mediatorSet = new List<Vertex<T>>();
            var connectedComs = new List<Community<T>>();
            while (connectedComs.Count < _graph.Communities.Count)
            {
                var n = orderedNodes[0];
                foreach (var community in _graph.GetIncidentCommunitiesOfVertex(n))
                {
                    if (!connectedComs.Contains(community))
                    {
                        if (!mediatorSet.Contains(n))
                        {
                            mediatorSet.Add(n);
                        }
                        connectedComs.Add(community);
                    }
                }
                orderedNodes.Remove(n);
            }

            foreach (var node in mediatorSet)
                if (node.Role == 0)
                {
                    node.Role = Role.Mediator;
                }

        }

        /// <summary>
        /// Extracts outermosts from a graph
        /// </summary>
        public void ExtractOutermosts()
        {
            foreach (var community in _graph.Communities)
            {
                var meanClosenessCentralityMeanInCommunity = _algorithms.GetCommunityClosenessCentralityMean(community);
                var standartDeviationClosenessCentrality = _algorithms.GetCommunityClosenessCentralityStandartDeviation(community);
                var thresholdForOutermosts = meanClosenessCentralityMeanInCommunity - (2 * standartDeviationClosenessCentrality); //mean - 2*standart deviation

                foreach (var node in community.CommunityVertices)
                    if (node.ClosenessCentralityInCommunity < thresholdForOutermosts)
                        node.Role = Role.Outermost;
            }
        }
    }
}
