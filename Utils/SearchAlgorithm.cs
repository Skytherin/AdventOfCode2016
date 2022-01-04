using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode2016.Utils
{
    public static class SearchAlgorithm
    {
        public static SearchData<TNode> BreadthFirstSearch<TNode>(TNode initial, TNode needle,
            Func<TNode, IEnumerable<TNode>> neighbors, Func<TNode, bool> valid)
        {
            return RunInternal(initial, needle, new BreadthFirstContainer<SearchData<TNode>>(), neighbors, valid);
        }

        public static SearchData<TNode> AStarSearch<TNode>(TNode initial, TNode needle,
            Func<TNode, IEnumerable<(int Cost, TNode Node)>> neighbors, Func<TNode, bool> valid,
            Func<TNode, int> estimationFunction)
        {
            var open = new PriorityQueue<SearchData<TNode>>(node => node.Cost + estimationFunction(node.Value));
            open.Enqueue(new(initial, 0));
            var closed = new Dictionary<TNode, int> { {initial, 0}};
            while (open.TryDequeue(out var current))
            {
                if (closed.TryGetValue(current.Value, out var other) && other <= current.Cost) continue;

                foreach (var neighbor in neighbors(current.Value)
                             .Where(it => valid(it.Node)))
                {
                    if (closed.TryGetValue(current.Value, out var other2) && other2 <= current.Cost) continue;
                    if (neighbor.Equals(needle)) return new(neighbor.Node, current.Cost + neighbor.Cost);
                    closed[neighbor.Node] = current.Cost + neighbor.Cost;
                    open.Enqueue(new(neighbor.Node, current.Cost + neighbor.Cost));
                }
            }

            throw new ApplicationException("Search result not found.");
        }

        private static SearchData<TNode> RunInternal<TNode>(TNode initial, TNode needle,
            IContainer<SearchData<TNode>> open, Func<TNode, IEnumerable<TNode>> neighbors, Func<TNode, bool> valid)
        {
            open.Add(new(initial, 0));
            var closed = new HashSet<TNode> { initial };
            while (open.TryRemove(out var current))
            {
                foreach (var neighbor in neighbors(current.Value)
                             .Where(valid)
                             .Where(neighbor => !closed.Contains(neighbor)))
                {
                    if (neighbor.Equals(needle)) return new (neighbor, current.Cost + 1);
                    closed.Add(neighbor);
                    open.Add(new(neighbor, current.Cost + 1));
                }
            }

            throw new ApplicationException("Search result not found.");
        }

        public record SearchData<T>(T Value, int Cost);

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
    }
}