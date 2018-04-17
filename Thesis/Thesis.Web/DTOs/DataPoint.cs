using System.Runtime.Serialization;

namespace Thesis.Web.DTOs
{
    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            this.label = label;
            this.y = y;
        }

        public string label;

        public double y;

    }
}