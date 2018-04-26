using Domain.DomainClasses;

namespace Domain.GraphClasses
{
    public class Edge<T> 
    {
        public Node<T> Node1 { get; set; }
        public Node<T> Node2 { get; set; }

        public double Weight { get; set; }

        public bool SelfLoop => Node1.Id == Node2.Id;

        public Edge(Node<T> node1, Node<T> node2, double weight)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
        }

        public Edge(Node<T> node1, Node<T> node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        public Edge()
        {
        }
    }
}
