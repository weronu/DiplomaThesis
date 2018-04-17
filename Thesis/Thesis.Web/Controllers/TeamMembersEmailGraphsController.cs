using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain.DTOs;
using Domain.Enums;
using Thesis.Services.Interfaces;
using Thesis.Web.DTOs;
using Thesis.Web.Models;
using Domain.GraphClasses;
using Graph.Algorithms;
using Newtonsoft.Json;
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
                else // file is imported
                {
                    if (model.FromDate == null || model.ToDate == null)
                    {
                        List<DateTime> startAndEndDateOfConversations = GetStartAndEndDateOfConversations(model.SelectedTeamMemberId.ToString(), true);
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

        private List<DateTime> GetStartAndEndDateOfConversations(string teamMemberId, bool isFileImported = false)
        {
            List<DateTime> listOfDates = new List<DateTime>();
            try
            {
                FetchListServiceResponse<DateTime> startAndEndOfConversation;
                if (isFileImported == false)
                {
                    startAndEndOfConversation = _graphService.FetchStartAndEndOfConversation(GetConnectionStringBasedOnSelectedMember(teamMemberId));
                }
                else
                {
                    startAndEndOfConversation = _graphService.FetchStartAndEndOfConversation(_importConnectionString);
                }


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

                if (model.FromDate != null || model.ToDate != null)
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
                return new HttpStatusCodeResult(500, e.Message);
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
                return new HttpStatusCodeResult(500, e.Message);
            }

            return View("GraphView_partial", model);
        }


        [HttpPost]
        public ActionResult CreateEgoNetwork(GraphViewModel graphViewModel, int egoNetworkCenterId)
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
                        size = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).size),
                        group = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).group),
                        shape = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).shape)
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
                    graphViewModel.GraphDto.nodes.First(x => x.id == egoNetworkCenterId).size = 45;
                }

            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(500, e.Message);
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

                if (graphViewModel.Graph.Communities.Count > 0)
                {
                    graphViewModel.Graph.Communities = new HashSet<Community<UserDto>>();
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

                graphViewModel.Graph.SetDegrees();
                List<NodeDto> nodes = graphViewModel.Graph.Nodes.Select(x => new NodeDto()
                {
                    id = x.Id,
                    label = x.NodeElement.Name,
                    group = x.CommunityId,
                    title = $"Node degree: {x.Degree}",
                    size = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).size)
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
                return new HttpStatusCodeResult(500, e.Message);
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
                        group = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).group),
                        title = $"Node degree: {x.Degree}",
                        size = GetNodeSizeBasedOnRole(x),
                        shape = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).shape)
                    }).ToList();
                    List<EdgeDto> edges = graphViewModel.Graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

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
                return new HttpStatusCodeResult(500, e.Message);
            }
            return View("GraphView_partial", graphViewModel);
        }

        private static int GetNodeSizeBasedOnRole(Node<UserDto> node)
        {
            switch (node.Role)
            {
                case Role.Leader:
                    return 25;
                case Role.Mediator:
                    return 17;
                case Role.Outermost:
                    return 10;
                case Role.Outsider:
                    return 9;
                default:
                    return 12;
            }
        }

        [HttpPost]
        public ActionResult FindBrokerage(GraphViewModel graphViewModel)
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

                FetchItemServiceResponse<Graph<UserDto>> response = _graphService.DetectBrokerageInGraph(graphViewModel.Graph);

                if (response.Succeeded)
                {
                    graphViewModel.Graph = response.Item;
                    FetchListServiceResponse<BrokerageDto> topTenBrokersResponse = _graphService.FetchTopTenBrokers(graphViewModel.Graph, GetConnectionStringBasedOnSelectedMember(graphViewModel.SelectedTeamMemberId.ToString()));

                    if (topTenBrokersResponse.Succeeded)
                    {
                        graphViewModel.BrokerageDto = topTenBrokersResponse.Items;
                        graphViewModel.BrokerageDetected = true;
                    }

                    List<NodeDto> nodes = graphViewModel.Graph.Nodes.Select(x => new NodeDto()
                    {
                        id = x.Id,
                        label = x.NodeElement.Name,
                        title = $"Node degree: {x.Degree}",
                        size = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).size),
                        group = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).group)
                    }).ToList();
                    List<EdgeDto> edges = graphViewModel.Graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

                    HashSet<BrokerageDto> topTenBrokers = topTenBrokersResponse.Items;
                    foreach (NodeDto node in (nodes.Where(x => topTenBrokers.Select(y => y.UserId).Contains(x.id))))
                    {
                        node.shape = "diamond";
                        node.size = 20;
                    }

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
                return new HttpStatusCodeResult(500, e.Message);
            }

            return View("GraphView_partial", graphViewModel);
        }

        [HttpPost]
        public ActionResult DrawBrokerageGraph(GraphViewModel graphViewModel)
        {
            try
            {
                FetchListServiceResponse<BrokerageDto> topTenBrokersResponse = _graphService.FetchTopTenBrokers(graphViewModel.Graph, GetConnectionStringBasedOnSelectedMember(graphViewModel.SelectedTeamMemberId.ToString()));

                if (topTenBrokersResponse.Succeeded)
                {
                    graphViewModel.BrokerageDto = topTenBrokersResponse.Items;
                    graphViewModel.BrokerageDetected = true;
                }
                HashSet<BrokerageDto> topTenBrokers = topTenBrokersResponse.Items;

                graphViewModel.DataPointDto = new DataPointDto
                {
                    DataPointsCoordinator = new List<DataPoint>(),
                    DataPointsGatepeeker = new List<DataPoint>(),
                    DataPointsItinerant = new List<DataPoint>(),
                    DataPointsLiaison = new List<DataPoint>(),
                    DataPointsRepresentative = new List<DataPoint>(),
                    DataPointsTotal = new List<DataPoint>()
                };

                foreach (BrokerageDto broker in topTenBrokers)
                {
                    graphViewModel.DataPointDto.DataPointsCoordinator.Add(new DataPoint(broker.Name, broker.Coordinator));
                    graphViewModel.DataPointDto.DataPointsGatepeeker.Add(new DataPoint(broker.Name, broker.Gatepeeker));
                    graphViewModel.DataPointDto.DataPointsItinerant.Add(new DataPoint(broker.Name, broker.Itinerant));
                    graphViewModel.DataPointDto.DataPointsLiaison.Add(new DataPoint(broker.Name, broker.Liaison));
                    graphViewModel.DataPointDto.DataPointsRepresentative.Add(new DataPoint(broker.Name, broker.Representative));
                    graphViewModel.DataPointDto.DataPointsTotal.Add(new DataPoint(broker.Name, broker.TotalBrokerageScore));
                }
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(500, e.Message);
            }
            return View("Graph2d_partial", graphViewModel);
        }
    }
}