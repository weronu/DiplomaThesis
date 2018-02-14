using System.Collections.Generic;
using Thesis.Web.DTOs;

namespace Thesis.Web.Models
{
    public class VisGraphViewModel
    {
        public List<NodeDto> nodes { get; set; }
        public List<EdgeDto> edges { get; set; }
    }
}