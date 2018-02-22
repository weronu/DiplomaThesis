using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain.DTOs;
using Domain.GraphClasses;
using Graph.Algorithms;
using Thesis.Services.Interfaces;
using Thesis.Web.DTOs;
using Thesis.Web.Models;

namespace Thesis.Web.Controllers
{
    public class TeamMembersEmailGraphsController : Controller
    {
        private const string defaultConnectionString = "GLEmailsDatabaseVeronika";
        public IGraphService _graphService;

        private static readonly List<TeamMemberDto> teamMembers = new List<TeamMemberDto>()
        {
            new TeamMemberDto() {Id = 1, Name = "Veronika Uhrova", ConnectionString = "GLEmailsDatabaseVeronika"},
            new TeamMemberDto() {Id = 2, Name = "Tibor Palatka", ConnectionString = "GLEmailsDatabaseTibor"},
            new TeamMemberDto() {Id = 3, Name = "Andrej Parimucha", ConnectionString = "GLEmailsDatabaseAndrej"},
            new TeamMemberDto() {Id = 4, Name = "Andrej Matejcik", ConnectionString = "GLEmailsDatabaseAdo"},
            new TeamMemberDto() {Id = 5, Name = "Explore whole team network", ConnectionString = "GLEmailsDatabase"}
        };

        private GraphViewModel model;

        
        public TeamMembersEmailGraphsController(IGraphService graphService)
        {
            _graphService = graphService;
        }

        public ActionResult Index()
        {
            Graph<UserDto> graph = _graphService.FetchEmailsGraph(defaultConnectionString);
            model = new GraphViewModel
            {
                TeamMembers = teamMembers
            };
            List<NodeDto> nodes = graph.Nodes.Select(x => new NodeDto() { id = x.Id, label = x.NodeElement.Name }).ToList();
            List<EdgeDto> edges = graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();
            model.Graph = graph;
            model.SelectedTeamMemberId = 1;
            model.visGraphViewModel = new VisGraphViewModel()
            {
                edges = edges,
                nodes = nodes
            };
            return View("TeamMembersEmailGraphsView", model);
        }

        [HttpGet]
        public ActionResult RenderGraphView()
        {
            
            return PartialView("GraphView", model);
        }

        [HttpPost]
        public ActionResult GetSelectedValue(string teamMemberId)
        {
            int id = int.Parse(teamMemberId);
            string connectionString = teamMembers.Where(i => i.Id == id).Select(x => x.ConnectionString).FirstOrDefault();
            model.SelectedTeamMemberId = id;

            Graph<UserDto> graph = _graphService.FetchEmailsGraph(connectionString);
            model.Graph = graph;

            List<NodeDto> nodes = graph.Nodes.Select(x => new NodeDto() { id = x.Id, label = x.NodeElement.Name }).ToList();
            List<EdgeDto> edges = graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

            model.visGraphViewModel = new VisGraphViewModel()
            {
                edges = edges,
                nodes = nodes
            };
            return PartialView("GraphView", model);
        }

        [HttpPost]
        public ActionResult FindCommunities(GraphViewModel graphViewModel)
        {
            

            return PartialView("GraphView", model);
        }

        [HttpPost]
        public ActionResult CreateEgoNetwork(GraphViewModel graphViewModel)
        {
            if (graphViewModel.Graph != null)
            {
                EgoNetwork egoNetwork = new EgoNetwork();

                HashSet<HashSet<Node<UserDto>>> subGraphs = egoNetwork.FindConectedSubgraphs(graphViewModel.Graph);

                TeamMemberDto selectedTeamMember = teamMembers.FirstOrDefault(x => x.Id == graphViewModel.SelectedTeamMemberId);
                if (selectedTeamMember == null)
                {
                    throw new Exception("Invalid selected team member.");
                }

                int egoNetworkCenterId = _graphService.FetchNodeIdByUserName(selectedTeamMember.Name, selectedTeamMember.ConnectionString);
                Node<UserDto> egoNetworkCenter = graphViewModel.Graph.GetNodeById(egoNetworkCenterId); 

                HashSet<Node<UserDto>> nodesWithMAximalDegreeInSubgraphsAximalDegreeInSubgraph = egoNetwork.GetNodesWithMaximalDegreeInSubgraphs(subGraphs, egoNetworkCenter);

                foreach (Node<UserDto> node in nodesWithMAximalDegreeInSubgraphsAximalDegreeInSubgraph)
                {
                    Edge<UserDto> newEdge = new Edge<UserDto>()
                    {
                        Node1 = egoNetworkCenter,
                        Node2 = node
                    };
                    graphViewModel.Graph.AddEdge(newEdge);
                }

                List<NodeDto> nodes = graphViewModel.Graph.Nodes.Select(x => new NodeDto() { id = x.Id, label = x.NodeElement.Name }).ToList();
                List<EdgeDto> edges = graphViewModel.Graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

                model.visGraphViewModel = new VisGraphViewModel()
                {
                    edges = edges,
                    nodes = nodes
                };
               
            }

            return PartialView("GraphView", graphViewModel);
        }


        [HttpPost]
        public ActionResult FindRoles(GraphViewModel graphViewModel)
        {


            return PartialView("GraphView", model);
        }
    }
}