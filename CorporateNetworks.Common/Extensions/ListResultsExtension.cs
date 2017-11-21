using System.Collections.Generic;
using System.Linq;
using CorporateNetworks.Common.Models;

namespace CorporateNetworks.Common.Extensions
{
    public static class ListResultsExtension
    {
        public static void BuildPathes(this List<WeightedResultModel> results, int [] pathes, int nodeToStart)
        {
            var queue = new Queue<int>();
            queue.Enqueue(nodeToStart);

            while (queue.Count > 0)
            {
                var parent = queue.Dequeue();

                var children = pathes
                    .Select((value, index) => new { Parent = value, Child = index })
                    .Where(n => n.Parent == parent)
                    .Select(n => n.Child);

                foreach (var child in children)
                {
                    var parentResult = results.First(r => r.Node == parent);
                    var childResult = results.First(r => r.Node == child);
                    childResult.Path.Insert(0, $"{parentResult.Path}=>");
                    queue.Enqueue(child);
                }
            }
        }
    }
}
