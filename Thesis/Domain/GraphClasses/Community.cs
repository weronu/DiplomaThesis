using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Domain.GraphClasses
{
    public class Community<T>
    {
        public int Id { get; set; }
        
        public double ClosenessCentralityMedian { get; set; }
        public double ClosenessCentralityMean { get; set; }
        public double ClosenessCentralityStandartDeviation { get; set; }

        [ScriptIgnore]
        public HashSet<Node<T>> CommunityNodes { get; set; }
    }
}
