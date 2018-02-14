using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain.DomainClasses;
using Domain.GraphClasses;
using Thesis.Services.Interfaces;
using Thesis.Web.DTOs;
using Thesis.Web.Models;

namespace Thesis.Web.Controllers
{
    public class TeamMembersEmailGraphsController : Controller
    {
        public IGraphService _graphService;

        private static readonly List<TeamMemberDto> teamMembers = new List<TeamMemberDto>()
        {
            new TeamMemberDto() {Id = 1, Name = "Veronika Uhrova", ConnectionString = "GLEmailsDatabaseVeronika"},
            new TeamMemberDto() {Id = 2, Name = "Tibor Palatka", ConnectionString = "GLEmailsDatabaseTibor"},
            new TeamMemberDto() {Id = 3, Name = "Andrej Parimucha", ConnectionString = "GLEmailsDatabaseAndrej"},
            new TeamMemberDto() {Id = 4, Name = "Andrej Matejcik", ConnectionString = "GLEmailsDatabaseAdo"},
        };

        readonly TeamMemberViewModel model = new TeamMemberViewModel
        {
            TeamMembers = teamMembers
        };

        public TeamMembersEmailGraphsController(IGraphService graphService)
        {
            _graphService = graphService;
        }

        private string defaultConnectionString = "GLEmailsDatabase";

        [HttpGet]
        public ActionResult Index()
        {
            Graph<User> emailsGraph = _graphService.FetchEmailsGraph(defaultConnectionString);

            List<NodeDto> nodes = emailsGraph.Nodes.Select(x => new NodeDto() { id = x.Id, label = x.NodeElement.Name }).ToList();
            List<EdgeDto> edges = emailsGraph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

            model.visGraphViewModel = new VisGraphViewModel()
            {
                edges = edges,
                nodes = nodes
            };
            return View("TeamMembersEmailGraphsView", model);
        }

        public JsonResult GetNodesAndEdges(string connectionString)
        {
            if (connectionString == null)
            {
                connectionString = "GLEmailsDatabase";
            }
            Graph<User> emailsGraph = _graphService.FetchEmailsGraph(connectionString);

            List<NodeDto> nodes = emailsGraph.Nodes.Select(x => new NodeDto() {id = x.Id, label = x.NodeElement.Name}).ToList();
            List<EdgeDto> edges = emailsGraph.Edges.Select(x => new EdgeDto() {from = x.Node1.Id, to = x.Node2.Id}).ToList();

            return Json(new { nodes, edges}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetSelectedValue(string teamMemberId)
        {
            int id = Int32.Parse(teamMemberId);
            string connectionString = teamMembers.Where(i => i.Id == id).Select(x => x.ConnectionString).FirstOrDefault();
            model.SelectedTeamMemberId = id;

            Graph<User> emailsGraph = _graphService.FetchEmailsGraph(connectionString);

            List<NodeDto> nodes = emailsGraph.Nodes.Select(x => new NodeDto() { id = x.Id, label = x.NodeElement.Name }).ToList();
            List<EdgeDto> edges = emailsGraph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

            model.visGraphViewModel = new VisGraphViewModel()
            {
                edges = edges,
                nodes = nodes
            };
            return PartialView("GraphView", model);
        }

    }
}