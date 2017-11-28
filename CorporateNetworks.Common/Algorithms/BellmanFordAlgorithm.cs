using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorporateNetworks.Common.Extensions;
using CorporateNetworks.Common.Models;

namespace CorporateNetworks.Common.Algorithms
{
    public class BellmanFordAlgorithm
    {
        public static IEnumerable<PathToNode> Run(List<Edge> edges, int nodesCount, int nodeToStart, out bool hasNegativeCycle)
        {
            var weights = Enumerable.Repeat(double.PositiveInfinity, nodesCount).ToArray();
            var pathes = Enumerable.Repeat(-1, nodesCount).ToArray();

            weights[nodeToStart] = 0;

            int nodeAccessibleFromNegativeCycle = -1;
            for (var i = 0; i < nodesCount; i++)
            {
                nodeAccessibleFromNegativeCycle = -1;
                foreach (var edge in edges)
                {
                    if (edge.Parent < double.PositiveInfinity && weights[edge.Parent] + edge.Weight < weights[edge.Child])
                    {
                        weights[edge.Child] = weights[edge.Parent] + edge.Weight;
                        pathes[edge.Child] = edge.Parent;
                        nodeAccessibleFromNegativeCycle = edge.Child;
                    }
                }
            }

            List<PathToNode> results;

            if (nodeAccessibleFromNegativeCycle != -1)
            {
                hasNegativeCycle = true;
                var startNodeInNegativeCycle = nodeAccessibleFromNegativeCycle;
                for (var i = 0; i < nodesCount; ++i)
                {
                    startNodeInNegativeCycle = pathes[startNodeInNegativeCycle];
                }

                var startNodeInNegativeCycleString = startNodeInNegativeCycle.ToString();
                var path = new StringBuilder(startNodeInNegativeCycleString);
                var nodeInNegativeCycle = startNodeInNegativeCycle;
                while (nodeInNegativeCycle != startNodeInNegativeCycle || path.Length == startNodeInNegativeCycleString.Length)
                {
                    nodeInNegativeCycle = pathes[nodeInNegativeCycle];
                    path.Insert(0, $"{nodeInNegativeCycle}=>");
                }

                results = new List<PathToNode>
                {
                    new PathToNode {Path = path}
                };
            }
            else
            {
                hasNegativeCycle = false;
                results = weights.Select((value, index) => new PathToNode { Node = index, Weight = value, Path = new StringBuilder(index.ToString()) }).ToList();

                results.ToList().BuildPathes(pathes, nodeToStart);
            }

            return results;
        }
    }
}
