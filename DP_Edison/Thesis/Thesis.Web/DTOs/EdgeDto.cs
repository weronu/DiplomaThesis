
namespace Thesis.Web.DTOs
{
    public class EdgeDto
    {
        // this class is used to visualize edges of graph with vis.js, so properties have to reflect properties from edges in vis.js (case sensitive!!!)
        public int from { get; set; }   
        public int to { get; set; }
    }
}