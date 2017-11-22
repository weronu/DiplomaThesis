using System.Collections.Generic;
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
            foreach (Vertex<T> vertex in _graph.Vertices)
            {
                if (vertex.Community == null)
                {
                    vertex.Role = Role.Outsider;
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

                foreach (Vertex<T> vertex in community.CommunityVertices)
                {
                    if (vertex.ClosenessCentralityInCommunity > thresholdForLeaders)
                        vertex.Role = Role.Leader;
                }
            }
        }

        /// <summary>
        /// Extracts mediators from a graph
        /// </summary>
        public void ExtractMediators(List<Vertex<T>> orderedNodes)
        {
            List<Vertex<T>> mediatorSet = new List<Vertex<T>>();
            List<Community<T>> connectedComs = new List<Community<T>>();
            while (connectedComs.Count < _graph.Communities.Count)
            {
                Vertex<T> n = orderedNodes[0];
                foreach (Community<T> community in _graph.GetIncidentCommunitiesOfVertex(n))
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

            foreach (Vertex<T> vertex in mediatorSet)
            {
                if (vertex.Role == 0)
                {
                    vertex.Role = Role.Mediator;
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

                foreach (Vertex<T> vertex in community.CommunityVertices)
                {
                    if (vertex.ClosenessCentralityInCommunity < thresholdForOutermosts)
                    {
                        vertex.Role = Role.Outermost;
                    }
                }
            }
        }
    }
}
