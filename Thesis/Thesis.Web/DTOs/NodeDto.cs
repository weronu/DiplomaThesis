using System.Drawing;

namespace Thesis.Web.DTOs
{
    public class NodeDto
    {
        // this class is used to visualize nodes of graph with vis.js, so properties have to reflect properties from nodes in vis.js (case sensitive!!!)
        public int id { get; set; }
        public string label { get; set; }
        public string color { get; set; }
        public string title { get; set; }
        public int size { get; set; }
        public int group { get; set; }
        public string shape { get; set; }
    }
}