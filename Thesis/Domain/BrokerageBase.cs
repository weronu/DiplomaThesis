using Domain.GraphClasses;

namespace Domain
{
    public class BrokerageBase<T>
    {
        public Node<T> NodeA { get; set; }
        public Node<T> NodeB { get; set; }
        public Node<T> NodeC { get; set; }
    }
}
