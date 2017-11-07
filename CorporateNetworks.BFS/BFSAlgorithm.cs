using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorporateNetworks.Common.Extensions;
using CorporateNetworks.Common.Models;

namespace CorporateNetworks.BFS
{
    public static class BfsAlgorithm
    {
        public static IEnumerable<ResultModel> Run(double[][] adjacencyMatrix, int nodeToStart)
        {
            var nodesCount = adjacencyMatrix.Length;
            var used = new bool[nodesCount];
            var pathes = Enumerable.Repeat(-1, nodesCount).ToArray();

            var nodesToCheck = new Queue<int>();

            nodesToCheck.Enqueue(nodeToStart);
            used[nodeToStart] = true;

            while (nodesToCheck.Count > 0)
            {
                var parent = nodesToCheck.Dequeue();
                for (int i = 0; i < nodesCount; i++)
                {
                    if (double.IsInfinity(adjacencyMatrix[parent][i]) || used[i]) continue;

                    used[i] = true;
                    nodesToCheck.Enqueue(i);
                    pathes[i] = parent;
                }
            }

            var results = pathes.Select((value, index) => new ResultModel { Node = index, Path = new StringBuilder(index.ToString()) }).ToList();

            results.BuildPathes(pathes, nodeToStart);

            return results;
        }
    }
}
