using System;
using System.Collections.Generic;
using System.Linq;
using CorporateNetworks.Common.Models;

namespace CorporateNetworks.Common.Generation
{
    public static class EdgeGeneration
    {
        private static List<WeightedEdge> graph;
        private static List<WeightedEdge> transposeGraph;

        public static List<WeightedEdge> GenerateWeightedEdges(int nodesCount, bool areWeightsPositive = false, bool areEdgesDirectional = true)
        {
            var edges = GenerateEdges(nodesCount, areEdgesDirectional).ToList();

            var random = new Random();

            foreach (var edge in edges)
            {
                if (areEdgesDirectional ||
                    double.IsInfinity(edges.First(e => e.Parent == edge.Child && e.Child == edge.Parent).Weight))
                {
                    edge.Weight = areWeightsPositive
                        ? random.NextDouble() * nodesCount
                        : nodesCount * (2 * random.NextDouble() - 1);
                }
                else
                {
                    edge.Weight = edges.First(e => e.Parent == edge.Child && e.Child == edge.Parent).Weight;
                }
            }

            return edges;
        }

        private static IEnumerable<WeightedEdge> GenerateEdges(int nodesCount, bool areEdgesDirectional)
        {
            graph = new List<WeightedEdge>();
            transposeGraph = new List<WeightedEdge>();
            var nodes = new HashSet<int>();

            var minEdgeCount = nodesCount - 1;
            var maxEdgeCount = nodesCount * (nodesCount - 1) / 2 + 1;

            var random = new Random();
            var edgeCount = random.Next(minEdgeCount, maxEdgeCount + 1);

            while (graph.Count < edgeCount || nodes.Count < nodesCount || !IsStrongConnected(graph, nodesCount))
            {
                var parent = random.Next(0, nodesCount);
                var child = random.Next(0, nodesCount);
                while (parent == child)
                {
                    child = random.Next(0, nodesCount);
                }

                var edge = areEdgesDirectional
                            ? graph.Find(e => e.Parent == parent && e.Child == child)
                            : graph.Find(e => e.Parent == parent && e.Child == child || e.Parent == child && e.Child == parent);

                if (edge == null)
                {
                    graph.Add(new WeightedEdge { Parent = parent, Child = child });
                    transposeGraph.Add(new WeightedEdge { Child = parent, Parent = child });
                    if (!areEdgesDirectional)
                    {
                        graph.Add(new WeightedEdge { Parent = child, Child = parent });
                        transposeGraph.Add(new WeightedEdge { Child = child, Parent = parent });
                    }
                    nodes.Add(parent);
                    nodes.Add(child);
                }
            }

            return graph;
        }

        private static bool IsStrongConnected(List<WeightedEdge> edges, int nodesCount)
        {
            graph = edges;
            transposeGraph = graph.Select(e => new WeightedEdge() {Child = e.Parent, Parent = e.Child}).ToList();
            var order = new List<int>();

            var used = new bool[nodesCount];
            for (var i = 0; i < nodesCount; i++)
            {
                if (!used[i])
                {
                    Dfs1(i, order, used);
                }
            }

            used = new bool[nodesCount];
            var componentCounts = 0;
            for (int i = 0; i < nodesCount; ++i)
            {
                int v = order[nodesCount - 1 - i];
                if (!used[v])
                {
                    Dfs2(v, used);
                    if (++componentCounts > 1)
                    {
                        return false;
                    }
                }
            }

            order.Clear();

            return true;
        }

        private static void Dfs1(int v, ICollection<int> order, IList<bool> used)
        {
            used[v] = true;
            foreach (var edge in graph.Where(e => e.Parent == v))
            {
                if (!used[edge.Child])
                {
                    Dfs1(edge.Child, order, used);
                }
            }
                
            order.Add(v);
        }

        private static void Dfs2(int v, IList<bool> used)
        {
            used[v] = true;
            foreach (var edge in transposeGraph.Where(e => e.Parent == v))
            {
                if (!used[edge.Child])
                {
                    Dfs2(edge.Child, used);
                }
            }
        }
    }
}
