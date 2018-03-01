using System.Collections.Generic;
using Domain.GraphClasses;

namespace Graph.Algorithms
{
    public class ShortestPathSet<T>
    {
        public Node<T> StartNode { get; set; }
        public Node<T> EndNode { get; set; }
        public HashSet<Node<T>> ShortestPath { get; set; }
    }
}
