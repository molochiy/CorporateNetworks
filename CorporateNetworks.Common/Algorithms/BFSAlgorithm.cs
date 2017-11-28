using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorporateNetworks.Common.Extensions;
using CorporateNetworks.Common.Models;

namespace CorporateNetworks.Common.Algorithms
{
    public static class BfsAlgorithm
    {
        public static IEnumerable<PathToNode> Run(double[][] adjacencyMatrix, int nodeToStart)
        {
            var nodesCount = adjacencyMatrix.Length;
            var weights = Enumerable.Repeat(double.PositiveInfinity, nodesCount).ToArray();
            var used = new bool[nodesCount];
            var pathes = Enumerable.Repeat(-1, nodesCount).ToArray();

            var nodesToCheck = new Queue<int>();

            nodesToCheck.Enqueue(nodeToStart);
            used[nodeToStart] = true;
            weights[nodeToStart] = 0;

            while (nodesToCheck.Count > 0)
            {
                var parent = nodesToCheck.Dequeue();
                for (int i = 0; i < nodesCount; i++)
                {
                    if (double.IsInfinity(adjacencyMatrix[parent][i]) || used[i]) continue;

                    used[i] = true;
                    nodesToCheck.Enqueue(i);
                    pathes[i] = parent;
                    weights[i] = weights[parent] + adjacencyMatrix[parent][i];
                }
            }

            var results = weights.Select((value, index) => new PathToNode { Node = index, Weight = value, Path = new StringBuilder(index.ToString()) }).ToList();

            results.BuildPathes(pathes, nodeToStart);

            return results;
        }
    }
}
