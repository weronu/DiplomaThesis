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

                    responseGraph = _graphService.FetchEmailsGraph(GetConnectionStringBasedOnSelectedMember(model.SelectedTeamMemberId.ToString()));
                }
                else
                {
                    responseGraph = _graphService.FetchEmailsGraph(_importConnectionString);
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

                return View(model);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
        public ActionResult GetSelectedValue(string teamMemberId)
        {
            int id = int.Parse(teamMemberId);
            string connectionString = GetConnectionStringBasedOnSelectedMember(teamMemberId);

            FetchItemServiceResponse<Graph<UserDto>> responseGraph = _graphService.FetchEmailsGraph(connectionString);


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

            GraphViewModel model = new GraphViewModel
            {
                TeamMembers = TeamMembers,
                SelectedTeamMemberId = id,
                Graph = responseGraph.Item,
                GraphDto = graphDto
            };


            return PartialView("GraphView_partial", model);
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
                return PartialView("GraphView_partial", graphViewModel);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
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
                    if (!communities.TryGetValue(kvp.Value, out List<int> nodeset))
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
                    size = 10,
                    //group = x.CommunityId
                }).ToList();
                List<EdgeDto> edges = graphViewModel.Graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

                GraphDto graphDto = new GraphDto
                {
                    nodes = nodes,
                    edges = edges
                };

                graphViewModel.GraphDto = graphDto;

                return PartialView("GraphView_partial", graphViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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

                if (graphViewModel.Graph.Communities == null)
                {
                    this.AddToastMessage("Error", "You have to find communities first!");
                    return PartialView("GraphView_partial", graphViewModel);
                }

                GraphAlgorithm<UserDto> algorithms = new GraphAlgorithm<UserDto>(graphViewModel.Graph);
                HashSet<ShortestPathSet<UserDto>> shortestPaths = algorithms.GetAllShortestPathsInGraph(graphViewModel.Graph.Nodes);

                //setting closeness centrality
                algorithms.SetClosenessCentralityForEachNode(shortestPaths);

                //setting closeness centrality for community
                algorithms.SetClosenessCentralityForEachNodeInCommunity(shortestPaths);

                //community closeness centrality mean and standart deviation
                algorithms.SetMeanClosenessCentralityForEachCommunity();
                algorithms.SetStandartDeviationForClosenessCentralityForEachCommunity();

                //cPaths for nCBC measure
                HashSet<ShortestPathSet<UserDto>> cPaths = algorithms.CPaths(shortestPaths);

                //setting nCBC for each node
                algorithms.SetNCBCForEachNode(cPaths);

                //setting DSCount for each node
                algorithms.SetDSCountForEachNode(cPaths);

                GraphRoleDetection<UserDto> roleDetection = new GraphRoleDetection<UserDto>(graphViewModel.Graph, algorithms);
                roleDetection.ExtractOutsiders();
                roleDetection.ExtractLeaders();
                roleDetection.ExtractOutermosts();

                //sorting nodes by their mediacy score
                HashSet<Node<UserDto>> sortedNodes = algorithms.OrderNodesByMediacyScore();
                roleDetection.ExtractMediators(sortedNodes);


                List<NodeDto> nodes = graphViewModel.Graph.Nodes.Select(x => new NodeDto()
                {
                    id = x.Id,
                    label = x.NodeElement.Name,
                    color = (graphViewModel.GraphDto.nodes.First(y => y.id == x.Id).color),
                    title = $"Node degree: {x.Degree}",
                    size = GetNodeSizeBasedOnRole(x)
                }).ToList();
                List<EdgeDto> edges = graphViewModel.Graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

                GraphDto graphDto = new GraphDto
                {
                    nodes = nodes,
                    edges = edges
                };

                graphViewModel.GraphDto = graphDto;

                this.AddToastMessage("Success", "Roles detected successfully.", ToastType.Success);
                return PartialView("GraphView_partial", graphViewModel);
            }
            catch (Exception e)
            {
                this.AddToastMessage("Error", e.Message, ToastType.Error);
                throw new Exception(e.Message);
            }
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