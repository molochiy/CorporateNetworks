using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorporateNetworks.Common.Extensions;
using CorporateNetworks.Common.Models;

namespace CorporateNetworks.Common.Algorithms
{
    public class DijkstraAlgorithm
    {
        public static IEnumerable<WeightedResultModel> Run(double[][] adjacencyMatrix, int nodeToStart)
        {
            var nodesCount = adjacencyMatrix.Length;
            var weights = Enumerable.Repeat(double.PositiveInfinity, nodesCount).ToArray();
            var used = new bool[nodesCount];
            var pathes = Enumerable.Repeat(-1, nodesCount).ToArray();

            weights[nodeToStart] = 0;

            for (var i = 0; i < nodesCount; i++)
            {
                var nodeWithLovestWeight = -1;
                for (var j = 0; j < nodesCount; j++)
                {
                    if (!used[j] && (nodeWithLovestWeight == -1 || weights[j] < weights[nodeWithLovestWeight]))
                    { 
                        nodeWithLovestWeight = j;
                    }
                }
                if (double.IsInfinity(weights[nodeWithLovestWeight])) break;

                used[nodeWithLovestWeight] = true;

                var adjacencyVector = adjacencyMatrix[nodeWithLovestWeight];
                for (var j = 0; j < adjacencyVector.Length; j++)
                {
                    if (!double.IsInfinity(adjacencyVector[j]) && weights[nodeWithLovestWeight] + adjacencyVector[j] < weights[j])
                    {
                        weights[j] = weights[nodeWithLovestWeight] + adjacencyVector[j];
                        pathes[j] = nodeWithLovestWeight;
                    }
                }
            }

            var results = weights.Select((value, index) => new WeightedResultModel { Node = index, Weight = value, Path = new StringBuilder(index.ToString()) }).ToList();

            results.ToList().BuildPathes(pathes, nodeToStart);

            return results;
        }
    }
}
