using System.Runtime.Serialization;

namespace Thesis.Web.DTOs
{
    public class DataPointDto
    {
        public DataPointDto(string x, double y)
        {
            X = x;
            Y = y;
        }

        [DataMember(Name = "x")]
        public string X;

        [DataMember(Name = "y")]
        public double Y;

    }
}