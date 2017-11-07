using System;
using System.Collections.Generic;
using System.Linq;
using CorporateNetworks.Common.Models;

namespace CorporateNetworks.Common.Generation
{
    public static class EdgeGeneration
    {
        private static List<Edge> graph;
        private static List<Edge> transposeGraph;

        public static List<Edge> GenerateEdges(int nodesCount)
        {
            graph = new List<Edge>();
            transposeGraph = new List<Edge>();
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

                var edge = graph.Find(e => e.Parent == parent && e.Child == child);

                if (edge == null)
                {
                    graph.Add(new Edge { Parent = parent, Child = child });
                    transposeGraph.Add(new Edge { Child = parent, Parent = child });
                    nodes.Add(parent);
                    nodes.Add(child);
                }
            }

            return graph;
        }

        public static List<WeightedEdge> GenerateWeightedEdges(int nodesCount, bool areWeightsPositive = false)
        {
            var edges = GenerateEdges(nodesCount);

            var random = new Random();

            var weightedEdges = edges.Select(e =>
                new WeightedEdge
                {
                    Child = e.Child,
                    Parent = e.Parent,
                    Weight = areWeightsPositive
                    ? random.NextDouble() * nodesCount
                    : nodesCount * (2 * random.NextDouble() - 1)
                });

            return weightedEdges.ToList();
        }

        private static bool IsStrongConnected(List<Edge> edges, int nodesCount)
        {
            graph = edges;
            transposeGraph = graph.Select(e => new Edge {Child = e.Parent, Parent = e.Child}).ToList();
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
