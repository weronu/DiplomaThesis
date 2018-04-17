using System.Collections.Generic;

namespace Thesis.Web.DTOs
{
    public class DataPointDto
    {
        public List<DataPoint> DataPointsCoordinator { get; set; }
        public List<DataPoint> DataPointsItinerant { get; set; }
        public List<DataPoint> DataPointsGatepeeker { get; set; }
        public List<DataPoint> DataPointsLiaison { get; set; }
        public List<DataPoint> DataPointsRepresentative { get; set; }
        public List<DataPoint> DataPointsTotal { get; set; }
    }
}