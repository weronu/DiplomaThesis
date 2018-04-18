using Domain.Enums;

namespace Domain.GraphClasses
{
    public class Node<T>
    {
        public int Id { get; set; }
        public int CommunityId { get; set; }

        public int Degree { get; set; }
        public Role Role { get; set; }
        public double ClosenessCentrality { get; set; }
        public double ClosenessCentralityInCommunity { get; set; }
        
        public int CBetweeness { get; set; }
        public double NormalizedCBC { get; set; }
        public double DSCount { get; set; }
        public double MediacyScore => NormalizedCBC * DSCount;

        public T NodeElement { get; set; }
        public Community<T> Community { get; set; }

        public Brokerage Brokerage { get; set; }

        public double EIIndex { get; set; }
        public double EffectiveSize { get; set; }
    }
}
