using System;

namespace Domain.DTOs
{
    public class BrokerageDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Liaison { get; set; }
        public int Itinerant { get; set; }
        public int Coordinator { get; set; }
        public int Gatepeeker { get; set; }
        public int Representative { get; set; }
        public int TotalBrokerageScore { get; set; }
    }
}