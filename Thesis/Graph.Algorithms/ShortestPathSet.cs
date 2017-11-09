using System.Collections.Generic;
using Domain.DomainClasses;
using Domain.GraphClasses;

namespace Graph.Algorithms
{
    public class ShortestPathSet<T> where T: DomainBase
    {
        public Vertex<T> StartNode { get; set; }
        public Vertex<T> EndNode { get; set; }
        public HashSet<Vertex<T>> ShortestPath { get; set; }
    }
}
