namespace Domain
{
    public class Brokerage
    {
        public int Liaison { get; set; }
        public int Itinerant { get; set; }
        public int Coordinator { get; set; }
        public int Gatepeeker { get; set; }
        public int Representative { get; set; }
        public int TotalBrokerageScore => Liaison + Itinerant + Coordinator + Gatepeeker + Representative;
    }
}
