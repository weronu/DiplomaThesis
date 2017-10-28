using System.Collections.Generic;
using Domain.DomainClasses;

namespace Domain.GraphClasses
{
    public class Graph
    {
        public HashSet<Vertex<User>> Vertices { get; set; }
        public HashSet<Edge<User>> Edges { get; set; }
        public Dictionary<int, HashSet<Vertex<User>>> GraphSet { get; set; }
    }
}
