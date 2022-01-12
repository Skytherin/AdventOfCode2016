using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace AdventOfCode2016.Utils
{
    public static class SearchAlgorithm
    {
        public static SearchData<TNode> BreadthFirstSearch<TNode>(TNode initial, Func<TNode, bool> needle,
            Func<TNode, IEnumerable<TNode>> neighbors)
        {
            return RunInternal(initial, needle, new BreadthFirstContainer<SearchData<TNode>>(), neighbors);
        }

        public static SearchData<TNode> PrioritySearch<TNode>(TNode initial, Func<TNode, bool> needle,
            Func<TNode, IEnumerable<TNode>> neighbors, Func<TNode, long> comparer)
        {
            return RunInternalNoCache(initial, needle, new PriorityContainer<SearchData<TNode>>(it => comparer(it.Node)), 
                neighbors);
        }

        public static SearchData<TNode> AStarSearch<TNode>(TNode initial, TNode needle,
            Func<TNode, IEnumerable<(long Cost, TNode Node)>> neighbors,
            Func<TNode, long> estimationFunction)
        {
            return AStarSearchAll(initial, it => it.Equals(needle), neighbors, estimationFunction).First();
        }

        public static SearchData<TNode> AStarSearch<TNode>(TNode initial, Func<TNode, bool> needle,
            Func<TNode, IEnumerable<(long Cost, TNode Node)>> neighbors,
            Func<TNode, long> estimationFunction)
        {
            return AStarSearchAll(initial, needle, neighbors, estimationFunction).First();
        }

        public static IEnumerable<SearchData<TNode>> AStarSearchAll<TNode>(TNode initial, Func<TNode, bool> needle,
            Func<TNode, IEnumerable<(long Cost, TNode Node)>> neighbors,
            Func<TNode, long> estimationFunction)
        {
            var open = new PriorityQueue<SearchData<TNode>>(node => node.Steps + estimationFunction(node.Node));
            open.Enqueue(new(initial, 0));
            var closed = new Dictionary<TNode, long>();
            while (open.TryDequeue(out var current))
            {
                if (closed.TryGetValue(current.Node, out var other) && other <= current.Steps) continue;
                closed[current.Node] = current.Steps;

                foreach (var neighbor in neighbors(current.Node))
                {
                    var totalCost = neighbor.Cost + current.Steps;
                    if (needle(neighbor.Node))
                    {
                        yield return new(neighbor.Node, totalCost);
                        continue;
                    }
                    open.Enqueue(new(neighbor.Node, totalCost));
                }
            }
        }

        private static SearchData<TNode> RunInternal<TNode>(TNode initial, Func<TNode, bool> needle,
            IContainer<SearchData<TNode>> open, Func<TNode, IEnumerable<TNode>> neighbors)
        {
            open.Add(new(initial, 0));
            var closed = new HashSet<TNode> { initial };
            while (open.TryRemove(out var current))
            {
                foreach (var neighbor in neighbors(current.Node)
                             .Where(neighbor => !closed.Contains(neighbor)))
                {
                    if (needle(neighbor)) return new (neighbor, current.Steps + 1);
                    closed.Add(neighbor);
                    open.Add(new(neighbor, current.Steps + 1));
                }
            }

            throw new ApplicationException("Search result not found.");
        }

        private static SearchData<TNode> RunInternalNoCache<TNode>(TNode initial, Func<TNode, bool> needle,
            IContainer<SearchData<TNode>> open, Func<TNode, IEnumerable<TNode>> neighbors)
        {
            open.Add(new(initial, 0));
            while (open.TryRemove(out var current))
            {
                if (needle(current.Node)) return current;
                foreach (var neighbor in neighbors(current.Node))
                {
                    open.Add(new(neighbor, current.Steps + 1));
                }
            }

            throw new ApplicationException("Search result not found.");
        }

        public record SearchData<T>(T Node, long Steps);

        private interface IContainer<T>
        {
            bool TryRemove(out T value);
            void Add(T value);
        }

        private class BreadthFirstContainer<T> : IContainer<T>
        {
            private readonly Queue<T> Actual = new();
            public bool TryRemove(out T value)
            {
                return Actual.TryDequeue(out value);
            }

            public void Add(T value)
            {
                Actual.Enqueue(value);
            }
        }

        private class PriorityContainer<T> : IContainer<T>
        {
            private readonly PriorityQueue<T> Actual;

            public PriorityContainer(Func<T, long> priorityFunction)
            {
                Actual = new PriorityQueue<T>(priorityFunction);
            }

            public bool TryRemove(out T value)
            {
                return Actual.TryDequeue(out value);
            }

            public void Add(T value)
            {
                Actual.Enqueue(value);
            }
        }
    }
}