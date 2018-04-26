using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Domain.DomainClasses;
using Domain.Enums;
using Domain.GraphClasses;

namespace FileManager
{
    public static class FileWriter
    {
        public static void CreateGephiFile(Graph<User> graph, string path, bool directed = false)
        {
            const string LeaderSize = "11.5";
            const string MediatorSize = "11.0";
            const string OutermostSize = "10.0";
            const string OutsiderSize = "9.5";
            const string DefaultSize = "10.5";
            const string Width = "w ";
            const string Height = "h ";
            const string Depth = "d ";

            try
            {
                int collorsCount = graph.Communities.Count;
                List<string> colors = new List<string>();
                for (int i = 0; i < collorsCount; i++)
                {
                    string color = $"#{StaticRandom.Instance.Next(0x1000000):X6}";
                    colors.Add(color);
                }

                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine("graph");
                    writer.WriteLine("[");
                    writer.WriteLine("Creator \"Gephi\"");
                    writer.WriteLine(directed ? "directed 1" : "directed 0");

                    //node
                    foreach (Node<User> node in graph.Nodes)
                    {

                        writer.WriteLine("node");
                        writer.WriteLine("[");
                        writer.WriteLine($"id {node.Id}");
                        writer.WriteLine($"label \"{node.Id}\"");
                        writer.WriteLine("graphics");
                        writer.WriteLine("[");

                        const int max = 400;
                        const int min = 100;
                        double x = StaticRandom.Instance.Next(0, max - 25);
                        double y = StaticRandom.Instance.Next(0, min - 15);

                        var Y = x.ToString(CultureInfo.InvariantCulture);
                        var X = y.ToString(CultureInfo.InvariantCulture);

                        writer.WriteLine($"x {X}");
                        writer.WriteLine($"y {Y}");
                        writer.WriteLine("z 0.0");

                        if (graph.Communities.Count > 0)
                        {
                            switch (node.Role)
                            {
                                case Role.Leader:
                                    writer.WriteLine(Width + LeaderSize);
                                    writer.WriteLine(Height + LeaderSize);
                                    writer.WriteLine(Depth + LeaderSize);
                                    writer.WriteLine($"fill \"{colors[node.CommunityId]}\"");
                                    break;
                                case Role.Outermost:
                                    writer.WriteLine(Width + OutermostSize);
                                    writer.WriteLine(Height + OutermostSize);
                                    writer.WriteLine(Depth + OutermostSize);
                                    writer.WriteLine($"fill \"{colors[node.CommunityId]}\"");
                                    break;
                                case Role.Mediator:
                                    writer.WriteLine(Width + MediatorSize);
                                    writer.WriteLine(Height + MediatorSize);
                                    writer.WriteLine(Depth + MediatorSize);
                                    writer.WriteLine($"fill \"{colors[node.CommunityId]}\"");
                                    break;
                                case Role.Outsider:
                                    writer.WriteLine(Width + OutsiderSize);
                                    writer.WriteLine(Height + OutsiderSize);
                                    writer.WriteLine(Depth + OutsiderSize);
                                    writer.WriteLine($"fill \"{colors[node.CommunityId]}\"");
                                    break;
                                default:
                                    writer.WriteLine(Width + DefaultSize);
                                    writer.WriteLine(Height + DefaultSize);
                                    writer.WriteLine(Depth + DefaultSize);
                                    writer.WriteLine($"fill \"{colors[node.CommunityId]}\"");
                                    break;
                            }

                            writer.WriteLine("]");
                            writer.WriteLine("]");
                        }
                        else
                        {
                            writer.WriteLine(Width + DefaultSize);
                            writer.WriteLine(Height + DefaultSize);
                            writer.WriteLine(Depth + DefaultSize);
                            writer.WriteLine($"fill \"#0000FF\""); // default - blue
                        }
                    }


                    int id = 0;
                    //edge
                    foreach (Edge<User> edge in graph.Edges)
                    {
                        writer.WriteLine("edge");
                        writer.WriteLine("[");
                        writer.WriteLine($"id {id}");
                        writer.WriteLine($"source {edge.Node1.Id}");
                        writer.WriteLine($"target {edge.Node2.Id}");
                        writer.WriteLine("value 1.0");
                        writer.WriteLine("fill \"#000000\"");
                        writer.WriteLine("]");
                        id++;
                    }
                    writer.WriteLine("]");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error message: {e}");
            }
        }
    }

    /// <summary>
    /// Class for generating unique random numbers.</summary>
    public static class StaticRandom
    {
        private static int seed;

        private static ThreadLocal<Random> threadLocal = new ThreadLocal<Random>
            (() => new Random(Interlocked.Increment(ref seed)));

        static StaticRandom()
        {
            seed = Environment.TickCount;
        }

        public static Random Instance { get { return threadLocal.Value; } }
    }
}
