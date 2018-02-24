using System.Collections.Generic;
using System.Linq;
using Domain.DomainClasses;
using Domain.GraphClasses;
using Thesis.Web.DTOs;

namespace Thesis.Web.Models
{
    public class VisGraphViewModel
    {
        public List<NodeDto> nodes { get; set; }
        public List<EdgeDto> edges { get; set; }

    }
}