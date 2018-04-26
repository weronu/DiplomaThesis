using System.Collections.Generic;

namespace Thesis.Web.DTOs
{
    public class GraphDto
    {
        public List<NodeDto> nodes { get; set; }
        public List<EdgeDto> edges { get; set; }
    }
}