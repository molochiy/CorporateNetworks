using System;
using System.Collections.Generic;
using System.Linq;
using CorporateNetworks.Common.Algorithms;
using CorporateNetworks.Common.Models;

namespace CorporateNetworks.Yen
{
    public class YenAlgorithm
    {
        public static IEnumerable<YenResultModel> Run(double[][] adjacencyMatrix, int nodeToStart, int nodeToEnd, int amountPaths)
        {
            var adjacencyMatrixCopy = adjacencyMatrix.Select(s => s.ToArray()).ToArray();
            var result = new List<YenResultModel>();

            var shortestWays = DijkstraAlgorithm.Run(adjacencyMatrixCopy, nodeToStart);
            var shortestWay = shortestWays.First(sw => sw.Node == nodeToEnd);
            result.Add(new YenResultModel
            {
                Iteration = 0,
                Path = shortestWay.Path,
                Weight = shortestWay.Weight
            });

            for (var k = 1; k < amountPaths; k++)
            {
                var minWeight = double.PositiveInfinity;
                Tuple<int, int> deletedEdge = null;
                WeightedResultModel newShortestWay = null;
                var nodesFromPath = shortestWay.Path.ToString().Split(new []{ "=>" }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                for (var i = 1; i < nodesFromPath.Count; i++)
                {
                    var deletedEdgeValue = adjacencyMatrixCopy[nodesFromPath[i - 1]][nodesFromPath[i]];
                    adjacencyMatrixCopy[nodesFromPath[i - 1]][nodesFromPath[i]] = double.PositiveInfinity;
                    adjacencyMatrixCopy[nodesFromPath[i]][nodesFromPath[i - 1]] = double.PositiveInfinity;

                    shortestWays = DijkstraAlgorithm.Run(adjacencyMatrixCopy, nodeToStart);

                    var shortestWayCandidate = shortestWays.FirstOrDefault(sw => sw.Node == nodeToEnd);
                    if (shortestWayCandidate != null && shortestWayCandidate.Weight < minWeight)
                    {
                        newShortestWay = shortestWayCandidate;
                        deletedEdge = new Tuple<int, int>(nodesFromPath[i - 1], nodesFromPath[i]);
                    }

                    adjacencyMatrixCopy[nodesFromPath[i - 1]][nodesFromPath[i]] = deletedEdgeValue;
                    adjacencyMatrixCopy[nodesFromPath[i]][nodesFromPath[i - 1]] = deletedEdgeValue;
                }

                if (newShortestWay != null)
                {
                    shortestWay = newShortestWay;
                    result.Add(new YenResultModel
                    {
                        Iteration = k,
                        Path = shortestWay.Path,
                        Weight = shortestWay.Weight
                    });
                    adjacencyMatrixCopy[deletedEdge.Item1][deletedEdge.Item2] = double.PositiveInfinity;
                    adjacencyMatrixCopy[deletedEdge.Item2][deletedEdge.Item1] = double.PositiveInfinity;
                }
            }

            return result;
        }
    }
}
