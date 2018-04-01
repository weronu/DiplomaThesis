using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Domain.DTOs;
using Domain.Enums;
using Thesis.Services.Interfaces;
using Thesis.Web.DTOs;
using Thesis.Web.Models;
using Domain.GraphClasses;
using Graph.Algorithms;
using Microsoft.Ajax.Utilities;
using Thesis.Services.ResponseTypes;


namespace Thesis.Web.Controllers
{
    public class TeamMembersEmailGraphsController : Controller
    {
        private readonly IGraphService _graphService;
        private readonly string _importConnectionString = "ThesisImportDatabase";

        private readonly List<TeamMemberDto> TeamMembers = new List<TeamMemberDto>()
        {
            new TeamMemberDto() {Id = 1, Name = "Veronika Uhrova", ConnectionString = "GLEmailsDatabaseVeronika"},
            new TeamMemberDto() {Id = 2, Name = "Tibor Palatka", ConnectionString = "GLEmailsDatabaseTibor"},
            new TeamMemberDto() {Id = 3, Name = "Andrej Parimucha", ConnectionString = "GLEmailsDatabaseAndrej"},
            new TeamMemberDto() {Id = 4, Name = "Andrej Matejcik", ConnectionString = "GLEmailsDatabaseAdo"},
            new TeamMemberDto() {Id = 5, Name = "Explore whole team network", ConnectionString = "GLEmailsDatabase"},
        };

        public TeamMembersEmailGraphsController(IGraphService graphService)
        {
            _graphService = graphService;
        }

        public ActionResult Index(GraphViewModel model)
        {
            try
            {
                FetchItemServiceResponse<Graph<UserDto>> responseGraph;

                if (model == null || model.FileImported == false)
                {
                    model = new GraphViewModel
                    {
                        TeamMembers = TeamMembers,
                        SelectedTeamMemberId = 1,
                    };

                    if (model.FromDate == null || model.ToDate == null)
                    {
                        List<DateTime> startAndEndDateOfConversations = GetStartAndEndDateOfConversations(model.SelectedTeamMemberId.ToString());
                        model.FromDate = startAndEndDateOfConversations.First().ToString("MM/dd/yyyy");
                        model.ToDate = startAndEndDateOfConversations.Last().ToString("MM/dd/yyyy");
                    }

                    DateTime from = DateTime.ParseExact(model.FromDate, "MM/dd/yyyy", null);
                    DateTime to = DateTime.ParseExact(model.ToDate, "MM/dd/yyyy", null);

                    responseGraph = _graphService.FetchEmailsGraph(GetConnectionStringBasedOnSelectedMember(model.SelectedTeamMemberId.ToString()), from, to);
                }
                else
                {
                    if (model.FromDate == null || model.ToDate == null)
                    {
                        List<DateTime> startAndEndDateOfConversations = GetStartAndEndDateOfConversations(model.SelectedTeamMemberId.ToString());
                        model.FromDate = startAndEndDateOfConversations.First().ToString("MM/dd/yyyy");
                        model.ToDate = startAndEndDateOfConversations.Last().ToString("MM/dd/yyyy");
                    }

                    DateTime from = DateTime.ParseExact(model.FromDate, "MM/dd/yyyy", null);
                    DateTime to = DateTime.ParseExact(model.ToDate, "MM/dd/yyyy", null);

                    responseGraph = _graphService.FetchEmailsGraph(_importConnectionString, from, to);
                }

                responseGraph.Item.SetDegrees();

                List<NodeDto> nodes = responseGraph.Item.Nodes.Select(x => new NodeDto()
                {
                    id = x.Id,
                    label = x.NodeElement.Name,
                    title = $"Node degree: {x.Degree}",
                    size = 10,
                    color = "#f5cbee"
                }).ToList();
                List<EdgeDto> edges = responseGraph.Item.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

                GraphDto graphDto = new GraphDto
                {
                    nodes = nodes,
                    edges = edges
                };

                model.Graph = responseGraph.Item;
                model.GraphDto = graphDto;
            }
            catch (Exception e)
            {
                this.AddToastMessage("Error", e.Message, ToastType.Error); 
            }
            return View(model);
        }

        private List<DateTime> GetStartAndEndDateOfConversations(string teamMemberId)
        {
            List<DateTime> listOfDates = new List<DateTime>();
            try
            {
                FetchListServiceResponse<DateTime> startAndEndOfConversation = _graphService.FetchStartAndEndOfConversation(GetConnectionStringBasedOnSelectedMember(teamMemberId));
                
                foreach (DateTime date in startAndEndOfConversation.Items)
                {
                    listOfDates.Add(date);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listOfDates;
        }

        private string GetConnectionStringBasedOnSelectedMember(string selectedTeamMemberId)
        {
            try
            {
                int id = int.Parse(selectedTeamMemberId);
                string connectionString = TeamMembers.Where(i => i.Id == id).Select(x => x.ConnectionString).FirstOrDefault();
                return connectionString;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }



        [HttpPost]
        public ActionResult GetSelectedValue(GraphViewModel model, string teamMemberId)
        {
            try
            {
                int id = int.Parse(teamMemberId);
                string connectionString = GetConnectionStringBasedOnSelectedMember(teamMemberId);

                if (model.FromDate == null || model.ToDate == null)
                {
                    List<DateTime> startAndEndDateOfConversations = GetStartAndEndDateOfConversations(teamMemberId);
                    model.FromDate = startAndEndDateOfConversations.First().ToString("MM/dd/yyyy");
                    model.ToDate = startAndEndDateOfConversations.Last().ToString("MM/dd/yyyy");
                }

                DateTime from = DateTime.ParseExact(model.FromDate, "MM/dd/yyyy", null);
                DateTime to = DateTime.ParseExact(model.ToDate, "MM/dd/yyyy", null);

                FetchItemServiceResponse<Graph<UserDto>> responseGraph = _graphService.FetchEmailsGraph(connectionString, from, to);

                responseGraph.Item.SetDegrees();

                List<NodeDto> nodes = responseGraph.Item.Nodes.Select(x => new NodeDto()
                {
                    id = x.Id,
                    label = x.NodeElement.Name,
                    title = $"Node degree: {x.Degree}",
                    size = 10,
                    color = "#f5cbee"
                }).ToList();
                List<EdgeDto> edges = responseGraph.Item.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

                GraphDto graphDto = new GraphDto
                {
                    nodes = nodes,
                    edges = edges
                };


                model.TeamMembers = TeamMembers;
                model.SelectedTeamMemberId = id;
                model.Graph = responseGraph.Item;
                model.GraphDto = graphDto;
            }
            catch (Exception e)
            {
                this.AddToastMessage("Error", e.Message, ToastType.Error);
            }

            return View("GraphView_partial", model);
        }

        [HttpPost]
        public ActionResult ApplyDateRange(string from, string to, string selectedTeamMemberId)
        {
            GraphViewModel model = new GraphViewModel();
            try
            {
                DateTime fromDate = DateTime.ParseExact(from, "MM/dd/yyyy", null);
                DateTime toDate = DateTime.ParseExact(to, "MM/dd/yyyy", null);


                string connectionString = GetConnectionStringBasedOnSelectedMember(selectedTeamMemberId);

                FetchItemServiceResponse<Graph<UserDto>> responseGraph = _graphService.FetchEmailsGraph(connectionString, fromDate, toDate);

                responseGraph.Item.SetDegrees();

                List<NodeDto> nodes = responseGraph.Item.Nodes.Select(x => new NodeDto()
                {
                    id = x.Id,
                    label = x.NodeElement.Name,
                    title = $"Node degree: {x.Degree}",
                    size = 10,
                    color = "#f5cbee"
                }).ToList();
                List<EdgeDto> edges = responseGraph.Item.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

                GraphDto graphDto = new GraphDto
                {
                    nodes = nodes,
                    edges = edges
                };

                model.TeamMembers = TeamMembers;
                model.SelectedTeamMemberId = Int32.Parse(selectedTeamMemberId);
                model.Graph = responseGraph.Item;
                model.GraphDto = graphDto;
                model.FromDate = fromDate.ToString("MM/dd/yyyy");
                model.ToDate = toDate.ToString("MM/dd/yyyy");

            }
            catch (Exception e)
            {
                this.AddToastMessage("Error", e.Message, ToastType.Error);
            }

            return View("GraphView_partial", model);
        }


        [HttpPost]
        public ActionResult CreateEgoNetwork(GraphViewModel graphViewModel)
        {
            try
            {
                if (graphViewModel.Graph != null)
                {
                    EgoNetwork egoNetwork = new EgoNetwork();
                    if (graphViewModel.Graph.Edges.Count != 0)
                    {
                        foreach (Edge<UserDto> edge in graphViewModel.Graph.Edges)
                        {
                            graphViewModel.Graph.CreateGraphSet(edge);
                        }
                    }

                    HashSet<HashSet<Node<UserDto>>> subGraphs = egoNetwork.FindConectedSubgraphs(graphViewModel.Graph);

                    TeamMemberDto selectedTeamMember = graphViewModel.TeamMembers.FirstOrDefault(x => x.Id == graphViewModel.SelectedTeamMemberId);
                    int egoNetworkCenterId;
                    if (selectedTeamMember == null && graphViewModel.FileImported)
                    {
                        egoNetworkCenterId = graphViewModel.Graph.Nodes.OrderByDescending(i => i.Degree).First().Id;
                    }
                    else if (selectedTeamMember != null && graphViewModel.FileImported == false)
                    {
                        egoNetworkCenterId = _graphService.FetchNodeIdByUserName(selectedTeamMember.Name, selectedTeamMember.ConnectionString).Item;
                    }
                    else
                    {
                        throw new Exception("Invalid selected team member.");
                    }

                    if (egoNetworkCenterId == 0 && selectedTeamMember != null) // default to node with biggest degree
                    {
                        
                        FetchItemServiceResponse<Node<UserDto>> fetchNodeWithBiggestDegree = _graphService.FetchNodeWithBiggestDegree(selectedTeamMember.ConnectionString, graphViewModel.Graph);
                        egoNetworkCenterId = fetchNodeWithBiggestDegree.Item.Id;
                    }

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

                    graphViewModel.Graph.SetDegrees();
                    List<NodeDto> nodes = graphViewModel.Graph.Nodes.Select(x => new NodeDto()
                    {
                        id = x.Id,
                        label = x.NodeElement.Name,
                        title = $"Node degree: {x.Degree}",
                        size = 10,
                        color = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).color)
                    }).ToList();
                    List<EdgeDto> edges = graphViewModel.Graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

                    GraphDto graphDto = new GraphDto
                    {
                        nodes = nodes,
                        edges = edges
                    };

                    graphViewModel.TeamMembers = TeamMembers;
                    graphViewModel.SelectedTeamMemberId = graphViewModel.SelectedTeamMemberId;
                    graphViewModel.Graph = graphViewModel.Graph;
                    graphViewModel.GraphDto = graphDto;

                    graphViewModel.GraphDto.nodes.First(x => x.id == egoNetworkCenterId).color = "#721549";
                }
                
            }
            catch (Exception e)
            {
                this.AddToastMessage("Error", e.Message, ToastType.Error);
            }
            return View("GraphView_partial", graphViewModel);
        }

        [HttpPost]
        public ActionResult FindCommunities(GraphViewModel graphViewModel)
        {
            try
            {
                if (graphViewModel.Graph.Edges.Count != 0)
                {
                    foreach (Edge<UserDto> edge in graphViewModel.Graph.Edges)
                    {
                        graphViewModel.Graph.CreateGraphSet(edge);
                    }
                }

                Dictionary<int, int> partition = LouvainCommunity.BestPartition(graphViewModel.Graph);
                Dictionary<int, List<int>> communities = new Dictionary<int, List<int>>();
                foreach (KeyValuePair<int, int> kvp in partition)
                {
                    List<int> nodeset;
                    if (!communities.TryGetValue(kvp.Value, out nodeset))
                    {
                        nodeset = communities[kvp.Value] = new List<int>();
                    }
                    nodeset.Add(kvp.Key);
                }
                graphViewModel.Graph.SetCommunities(communities);

                int collorsCount = graphViewModel.Graph.Communities.Count;
                List<string> colors = new List<string>();
                for (int i = 0; i < collorsCount; i++)
                {
                    string color = $"#{StaticRandom.Instance.Next(0x1000000):X6}";
                    colors.Add(color);
                }

                graphViewModel.Graph.SetDegrees();
                List<NodeDto> nodes = graphViewModel.Graph.Nodes.Select(x => new NodeDto()
                {
                    id = x.Id,
                    label = x.NodeElement.Name,
                    color = colors[x.CommunityId],
                    title = $"Node degree: {x.Degree}",
                    size = 10
                }).ToList();
                List<EdgeDto> edges = graphViewModel.Graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

                GraphDto graphDto = new GraphDto
                {
                    nodes = nodes,
                    edges = edges
                };

                graphViewModel.GraphDto = graphDto;
            }
            catch (Exception e)
            {
                this.AddToastMessage("Error", e.Message, ToastType.Error);
            }
            return View("GraphView_partial", graphViewModel);
        }

        [HttpPost]
        public ActionResult FindRoles(GraphViewModel graphViewModel)
        {
            try
            {
                if (graphViewModel.Graph.Edges.Count != 0 && graphViewModel.Graph.GraphSet.Count == 0)
                {
                    foreach (Edge<UserDto> edge in graphViewModel.Graph.Edges)
                    {
                        graphViewModel.Graph.CreateGraphSet(edge);
                    }
                }

                FetchItemServiceResponse<Graph<UserDto>> response = _graphService.DetectRolesInGraph(graphViewModel.Graph);
                if (response.Succeeded)
                {
                    graphViewModel.Graph = response.Item;

                    List<NodeDto> nodes = graphViewModel.Graph.Nodes.Select(x => new NodeDto()
                    {
                        id = x.Id,
                        label = x.NodeElement.Name,
                        color = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).color),
                        title = $"Node degree: {x.Degree}",
                        size = GetNodeSizeBasedOnRole(x)
                    }).ToList();
                    List<EdgeDto> edges = graphViewModel.Graph.Edges.Select(x => new EdgeDto() {from = x.Node1.Id, to = x.Node2.Id}).ToList();

                    GraphDto graphDto = new GraphDto
                    {
                        nodes = nodes,
                        edges = edges
                    };

                    graphViewModel.GraphDto = graphDto;
                }
            }
            catch (Exception e)
            {
                this.AddToastMessage("Error", e.Message, ToastType.Error);
            }
            return View("GraphView_partial", graphViewModel);
        }

        private static int GetNodeSizeBasedOnRole(Node<UserDto> node)
        {
            switch (node.Role)
            {
                case Role.Leader:
                    return 20;
                case Role.Mediator:
                    return 16;
                case Role.Outermost:
                    return 10;
                case Role.Outsider:
                    return 9;
                default:
                    return 12;
            }
        }
    }

    public static class StaticRandom
    {
        private static int seed;

        private static readonly ThreadLocal<Random> threadLocal = new ThreadLocal<Random>
            (() => new Random(Interlocked.Increment(ref seed)));

        static StaticRandom()
        {
            seed = Environment.TickCount;
        }

        public static Random Instance => threadLocal.Value;
    }
}