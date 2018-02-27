﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Domain.DTOs;
using Thesis.Services.Interfaces;
using Thesis.Web.DTOs;
using Thesis.Web.Models;
using Domain.GraphClasses;
using Graph.Algorithms;

namespace Thesis.Web.Controllers
{
    public class TeamMembersEmailGraphsController : Controller
    {
        private readonly IGraphService _graphService;

        private readonly List<TeamMemberDto> TeamMembers = new List<TeamMemberDto>()
        {
            new TeamMemberDto() {Id = 1, Name = "Veronika Uhrova", ConnectionString = "GLEmailsDatabaseVeronika"},
            new TeamMemberDto() {Id = 2, Name = "Tibor Palatka", ConnectionString = "GLEmailsDatabaseTibor"},
            new TeamMemberDto() {Id = 3, Name = "Andrej Parimucha", ConnectionString = "GLEmailsDatabaseAndrej"},
            new TeamMemberDto() {Id = 4, Name = "Andrej Matejcik", ConnectionString = "GLEmailsDatabaseAdo"},
            new TeamMemberDto() {Id = 5, Name = "Explore whole team network", ConnectionString = "GLEmailsDatabase"}
        };
      

        public TeamMembersEmailGraphsController(IGraphService graphService)
        {
            _graphService = graphService;
        }
        public ActionResult Index()
        {
            try
            {
                GraphViewModel model = new GraphViewModel
                {
                    TeamMembers = TeamMembers,
                    SelectedTeamMemberId = 1,
                };

                Graph<UserDto> graph = _graphService.FetchEmailsGraph(GetConnectionStringBasedOnSelectedMember(model.SelectedTeamMemberId.ToString()));

                List<NodeDto> nodes = graph.Nodes.Select(x => new NodeDto() { id = x.Id, label = x.NodeElement.Name }).ToList();
                List<EdgeDto> edges = graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

                GraphDto graphDto = new GraphDto
                {
                    nodes = nodes,
                    edges = edges
                };

                model.Graph = graph;
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

            

            Graph<UserDto> graph = _graphService.FetchEmailsGraph(connectionString);
            

            List<NodeDto> nodes = graph.Nodes.Select(x => new NodeDto() { id = x.Id, label = x.NodeElement.Name }).ToList();
            List<EdgeDto> edges = graph.Edges.Select(x => new EdgeDto() { from = x.Node1.Id, to = x.Node2.Id }).ToList();

            GraphDto graphDto = new GraphDto
            {
                nodes = nodes,
                edges = edges
            };

            GraphViewModel model = new GraphViewModel
            {
                TeamMembers = TeamMembers,
                SelectedTeamMemberId = id,
                Graph = graph,
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

                List<NodeDto> nodes = graphViewModel.Graph.Nodes.Select(x => new NodeDto() { id = x.Id, label = x.NodeElement.Name, color = colors[x.CommunityId] }).ToList();
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

            return PartialView("GraphView_partial");
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