using Domain.DomainClasses;

namespace Domain.GraphClasses
{
    public class Vertex<T> where T : DomainBase
    {
        public int Id { get; set; }
    }
}
