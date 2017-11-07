using System.Collections.Generic;
using System.Linq;
using CorporateNetworks.Common.Models;

namespace CorporateNetworks.Common.Extensions
{
    public static class ListEdgesExtension
    {
        public static IEnumerable<Edge> GetNotDirectionedEdges(this List<Edge> edges)
        {
            var notDirectionedEdges = edges.ToList();
            notDirectionedEdges.AddRange(edges.Select(e => new Edge { Child = e.Parent, Parent = e.Child }).ToList());

            return notDirectionedEdges;
        }

        public static IEnumerable<WeightedEdge> GetNotDirectionedEdges(this List<WeightedEdge> edges)
        {
            var notDirectionedEdges = edges.ToList();
            notDirectionedEdges.AddRange(edges.Select(e => new WeightedEdge { Child = e.Parent, Parent = e.Child, Weight = e.Weight }).ToList());

            return notDirectionedEdges;
        }

        public static double[][] ToAdjacencyMatrix(this List<Edge> edges)
        {
            var nodesCount = edges.GetNodesCount();
            var adjacencyMatrix = Enumerable.Range(0, nodesCount)
                .Select(x => Enumerable.Repeat(double.PositiveInfinity, nodesCount).ToArray())
                .ToArray();

            foreach (var edge in edges)
            {
                adjacencyMatrix[edge.Parent][edge.Child] = 1;
            }

            return adjacencyMatrix;
        }

        public static double[][] ToAdjacencyMatrix(this List<WeightedEdge> edges)
        {
            var nodesCount = edges.Cast<Edge>().ToList().GetNodesCount();
            var adjacencyMatrix = Enumerable.Range(0, nodesCount)
                .Select(x => Enumerable.Repeat(double.PositiveInfinity, nodesCount).ToArray())
                .ToArray();

            foreach (var edge in edges)
            {
                adjacencyMatrix[edge.Parent][edge.Child] = edge.Weight;
            }

            return adjacencyMatrix;
        }

        public static int GetNodesCount(this List<Edge> edges)
        {
            var nodes = edges.Select(e => e.Parent).Distinct().ToList();
            nodes.AddRange(edges.Select(e => e.Child).Distinct().ToList());

            return nodes.Distinct().Count();
        }
    }
}
