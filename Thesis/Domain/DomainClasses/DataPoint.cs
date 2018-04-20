using System;
using System.Runtime.Serialization;

namespace Domain.DomainClasses
{
    [DataContract]
    public class DataPoint
    {
        [DataMember(Name = "label")]
        public string label { get; set; }

        [DataMember(Name = "y")]
        public double y { get; set; }
    }
}
