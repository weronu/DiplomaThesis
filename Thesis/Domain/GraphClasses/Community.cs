using System.Collections.Generic;
using Domain.DomainClasses;

namespace Domain.GraphClasses
{
    public class Community<T> where T: DomainBase 
    {
        public int Id { get; set; }
        public HashSet<Vertex<T>> CommunityVertices { get; set; }
        public Vertex<T> CentralVertex { get; set; }

        public double ClosenessCentralityMedian { get; set; }
        public double ClosenessCentralityMean { get; set; }
        public double ClosenessCentralityStandartDeviation { get; set; }

        public Community()
        {
            
        }
    }
}
