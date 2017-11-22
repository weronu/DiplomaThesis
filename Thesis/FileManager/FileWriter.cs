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
        public static void CreateGephiFile(Graph<User> graph, string path, bool directed)
        {

            try
            {
                List<string> colors = new List<string>();
                for (int i = 0; i < 5; i++)
                {
                    Random random = new Random();
                    var color = $"#{StaticRandom.Instance.Next(0x1000000):X6}";
                    colors.Add(color);
                }

                using (var writer = new StreamWriter(path))
                {
                    writer.WriteLine("graph");
                    writer.WriteLine("[");
                    writer.WriteLine("Creator \"Gephi\"");
                    writer.WriteLine(directed ? "directed 1" : "directed 0");

                    //node
                    foreach (var node in graph.Vertices)
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

                        switch (node.Role)
                        {
                            case Role.Leader:
                                writer.WriteLine("w 10.7");
                                writer.WriteLine("h 10.7");
                                writer.WriteLine("d 10.7");
                                writer.WriteLine($"fill \"{colors[node.CommunityId]}\"");
                                break;
                            case Role.Outermost:
                                writer.WriteLine("w 10.0");
                                writer.WriteLine("h 10.0");
                                writer.WriteLine("d 10.0");
                                writer.WriteLine($"fill \"{colors[node.CommunityId]}\"");
                                break;
                            case Role.Mediator:
                                writer.WriteLine("w 10.4");
                                writer.WriteLine("h 10.4");
                                writer.WriteLine("d 10.4");
                                writer.WriteLine($"fill \"{colors[node.CommunityId]}\"");
                                break;
                            case Role.Outsider:
                                writer.WriteLine($"fill \"{colors[node.CommunityId]}\"");
                                break;
                            default:
                                writer.WriteLine("w 10.2");
                                writer.WriteLine("h 10.2");
                                writer.WriteLine("d 10.2");
                                writer.WriteLine($"fill \"{colors[node.CommunityId]}\"");
                                break;
                        }

                        writer.WriteLine("]");
                        writer.WriteLine("]");
                    }

                    var id = 0;
                    //edge
                    foreach (Edge<User> edge in graph.Edges)
                    {
                        writer.WriteLine("edge");
                        writer.WriteLine("[");
                        writer.WriteLine($"id {id}");
                        writer.WriteLine($"source {edge.Vertex1.Id}");
                        writer.WriteLine($"target {edge.Vertex2.Id}");
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

        public static void CreateFile(Graph<User> graph, string path)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    
                    foreach (KeyValuePair<int, HashSet<Vertex<User>>> graphSet in graph.GraphSet)
                    {
                        string oneLine = "";
                        oneLine = oneLine + graphSet.Key + ",";
                        foreach (Vertex<User> node in graphSet.Value)
                        {
                            oneLine = oneLine + node.Id + ",";
                        }
                        oneLine = oneLine.Remove(oneLine.Length - 1);
                        writer.WriteLine(oneLine);
                    }
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
