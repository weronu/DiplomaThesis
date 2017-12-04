using System.Collections.Generic;
using Domain.DomainClasses;

namespace Domain.GraphClasses
{
    public class Community<T> where T: DomainBase 
    {
        public int Id { get; set; }
        public HashSet<Node<T>> CommunityNodes { get; set; }
        public Node<T> Centralnode { get; set; }

        public double ClosenessCentralityMedian { get; set; }
        public double ClosenessCentralityMean { get; set; }
        public double ClosenessCentralityStandartDeviation { get; set; }

        public Community(int id, HashSet<Node<T>> communityNodes)
        {
            Id = id;
            CommunityNodes = communityNodes;
        }
    }
}
