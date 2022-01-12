using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day24
{
    [UsedImplicitly]
    public class Day24: IAdventOfCode
    {
        private const int Wall = -2;
        private const int Space = -1;

        public IReadOnlyDictionary<Position, int> Parse(string s) =>
            s.Lines().WithIndices().SelectMany(row => row.Value.WithIndices().Select(col => (row.Index, col.Index,
                        col.Value switch
                        {
                            '#' => Wall,
                            '.' => Space,
                            _ => Convert.ToInt32($"{col.Value}")
                        }
                    )))
                .ToDictionary(it => new Position(it.Item1, it.Item2), it => it.Item3);

        public void Run()
        {
            Do1(Parse(this.Example())).Should().Be(14);
            Do1(Parse(this.Input())).Should().Be(518);
            Do2(Parse(this.Input())).Should().Be(716);
        }

        private long Do1(IReadOnlyDictionary<Position, int> input)
        {
            var temp = Compress(input);
            var keys = input.Values.Where(it => it >= 1).ToHashSet() as IReadOnlySet<int>;
            var initial = new Day24State(0, new List<int>(), 0);
            var temp2 = SearchAlgorithm.PrioritySearch(initial, node => node.Keys.Count == keys.Count, 
                node => Neighbors(node, keys, temp, false), state => state.Cost);
            return temp2.Node.Cost;
        }

        private long Do2(IReadOnlyDictionary<Position, int> input)
        {
            var temp = Compress(input);
            var keys = input.Values.Where(it => it >= 1).ToHashSet() as IReadOnlySet<int>;
            var initial = new Day24State(0, new List<int>(), 0);
            var temp2 = SearchAlgorithm.PrioritySearch(initial, node => node.Keys.Count == keys.Count && node.Current == 0,
                node => Neighbors(node, keys, temp, true), state => state.Cost);
            return temp2.Node.Cost;
        }

        private IEnumerable<Day24State> Neighbors(
            Day24State node,
            IReadOnlySet<int> allKeys, IReadOnlyDictionary<(int, int), long> map, bool goBackTo0)
        {
            foreach (var k in allKeys.Except(node.Keys))
            {
                yield return new Day24State(k, node.Keys.Append(k).ToList(), node.Cost + map[(node.Current, k)]);
            }

            if (goBackTo0 && allKeys.Count == node.Keys.Count && node.Current != 0)
            {
                yield return new Day24State(0, node.Keys, node.Cost + map[(node.Current, 0)]);
            }
        }

        private IReadOnlyDictionary<(int, int), long> Compress(IReadOnlyDictionary<Position, int> map)
        {
            var result = new Dictionary<(int, int), long>();
            foreach (var pair in map.Where(it => it.Value >= 0).Choose(2))
            {
                var k1 = pair[0].Key;
                var k2 = pair[1].Key;
                var v1 = pair[0].Value;
                var v2 = pair[1].Value;
                var c = SearchAlgorithm.AStarSearch(k1, k2, it => Neighbors(map, it), it => it.ManhattanDistance(k2)).Steps;
                result.Add((v1, v2), c);
                result.Add((v2, v1), c);
            }

            return result;
        }

        private IEnumerable<(long Cost, Position Node)> Neighbors(IReadOnlyDictionary<Position, int> map, Position it)
        {
            foreach (var neighbor in it.Orthogonal()
                         .Where(n => map.TryGetValue(n, out var temp) && temp > Wall)
                     )
            {
                yield return (1, neighbor);
            }
        }
    }

    public record Day24State(int Current, IReadOnlyList<int> Keys, long Cost);
}