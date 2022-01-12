using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day22
{
    [UsedImplicitly]
    public class Day22: IAdventOfCode
    {
        private Dictionary<Position, Day22NodeContents> Parse(string input)
        {
            return input.Lines().Where(it => !string.IsNullOrWhiteSpace(it))
                .Select(StructuredRx.Parse<Day22NodeRx>)
                .ToDictionary(it => new Position(it.Y, it.X), it =>
                {
                    var item = new Day22NodeContents(it.Size, it.Used);
                    item.Avail().Should().Be(it.Avail);
                    return item;
                });
        }

        public void Run()
        {
            //Console.WriteLine();
            //var input = Parse(this.Input());
            //var y = 0L;
            //foreach (var (key, value) in input.OrderBy(it => it.Key.Y).ThenBy(it => it.Key.X))
            //{
            //    if (key.Y != y)
            //    {
            //        Console.WriteLine();
            //        y = key.Y;
            //    }
            //    if (key.X > 0) Console.Write("  ");
            //    Console.Write($"{value.Used:##}/{value.Size:##}");

            //}

            //Do1(input.Values.ToList()).Should().Be(981);
            //Console.WriteLine("***");

            //Do2(Parse(this.Example())).Should().Be(7);
            //Console.WriteLine("***");
            //Do2(input).Should().Be(0);
        }

        private long Do1(List<Day22NodeContents> input)
        {
            return input.Choose(2).Select(it => (Viable(it[0], it[1]) ? 1 : 0) + (Viable(it[1], it[0]) ? 1 : 0)).Sum();
        }

        private long Do2(IReadOnlyDictionary<Position, Day22NodeContents> input)
        {
            Cache.Clear();
            var target = input.Keys.Where(i => i.Y == 0).MaxBy(i => i.X);
            var initial = new Day22Space(input, target, new List<Position>(), new List<Position>{target}, 0);

            var result = SearchAlgorithm.PrioritySearch(initial, 
                n => n.GoalTail[^1].Equals(Position.Zero) && n.CursorTail.Count == 0,
                Neighbors, PriorityFunction);

            Console.WriteLine(result.Node.GoalTail.Select(it => $"{it}").Join(";"));

            return result.Node.Cost;
        }

        private int Compare(Day22Space a, Day22Space b)
        {
            var cursorTail = a.CursorTail.Count.CompareTo(b.CursorTail.Count);
            var cost = a.Cost.CompareTo(b.Cost);
            var manhattan = a.GoalTail[^1].ManhattanDistance().CompareTo(b.GoalTail[^1].ManhattanDistance());
            
            if (cursorTail != 0) return cursorTail;
            if (cost != 0) return cost;
            if (manhattan != 0) return manhattan;
            return -1;
        }

        private long PriorityFunction(Day22Space space)
        {
            return space.CursorTail.Count;
        }

        private readonly Dictionary<Position, long> Cache = new();

        private IEnumerable<Day22Space> Neighbors(Day22Space space)
        {
            if (space.CursorTail.Count == 0)
            {
                foreach (var nextKey in space.GoalTail[^1].Orthogonal()
                             .Where(nextKey => space.Nodes.ContainsKey(nextKey))
                             .Where(nextKey => space.Nodes[nextKey].Size >= space.Nodes[space.GoalTail[^1]].Used)
                             .Where(nextKey => !space.GoalTail.Contains(nextKey)))
                {
                    yield return new Day22Space(space.Nodes, nextKey,
                        new List<Position> { space.CursorPosition }, space.GoalTail, space.Cost + 1);
                }
            }
            else
            {
                if (space.CursorTail.Count == 1)
                {
                    if (Cache.TryGetValue(space.CursorPosition, out var temp) && temp <= space.Cost)
                    {
                        yield break;
                    }
                }
                else
                {
                    if (Cache.TryGetValue(space.CursorTail[1], out var temp) && temp <= space.Cost)
                    {
                        yield break;
                    }
                }
                
                var tailPosition = space.CursorTail[^1];
                var tail = space.Nodes[tailPosition];
                var cursor = space.Nodes[space.CursorPosition];

                if (cursor.Size < tail.Used) yield break;

                if (cursor.Avail() >= tail.Used)
                {
                    // move from the end of the tail into the cursor
                    var nodes = space.Nodes.ToDictionary(it => it.Key, it => it.Value);
                    nodes[space.CursorPosition] = new Day22NodeContents(cursor.Size, cursor.Used + tail.Used);
                    nodes[tailPosition] = new Day22NodeContents(tail.Size, 0);

                    if (space.CursorTail.Count == 1)
                    {
                        Cache[space.CursorPosition] = space.Cost;
                        // We can move the goal forward to the cursor position
                        yield return new Day22Space(nodes, space.CursorPosition,
                            new List<Position>(), space.GoalTail.Append(space.CursorPosition).ToList(), space.Cost);
                    }
                    else
                    {
                        yield return new Day22Space(nodes, tailPosition,
                            space.CursorTail.Take(space.CursorTail.Count - 1).ToList(), space.GoalTail, space.Cost);
                    }
                }

                else if (space.Nodes[space.CursorPosition].Used > 0)
                {
                    foreach (var otherKey in space.CursorPosition.Orthogonal()
                                 .Where(other => space.Nodes.ContainsKey(other))
                                 .Where(other => space.Nodes[other].Size >= cursor.Used)
                                 .Where(other => !space.CursorTail.Contains(other)))
                    {
                        yield return new Day22Space(space.Nodes, otherKey,
                            space.CursorTail.Append(space.CursorPosition).ToList(), space.GoalTail, space.Cost + 1);
                    }
                }
            }
        }

        private bool Viable(Day22NodeContents a, Day22NodeContents b)
        {
            return a.Used != 0 && a.Used <= b.Avail();
        }
    }

    public class Day22Space
    {
        public Day22Space(IReadOnlyDictionary<Position, Day22NodeContents> nodes,
            Position cursorPosition,
            IReadOnlyList<Position> cursorTail,
            IReadOnlyList<Position> goalTail,
            int cost)
        {
            Nodes = nodes;
            CursorPosition = cursorPosition;
            CursorTail = cursorTail;
            GoalTail = goalTail;
            Cost = cost;
        }

        public IReadOnlyDictionary<Position, Day22NodeContents> Nodes { get; init; }
        public Position CursorPosition { get; init; }
        public IReadOnlyList<Position> CursorTail { get; init; }
        public IReadOnlyList<Position> GoalTail { get; init; }
        public int Cost { get; init; }

        public override int GetHashCode()
        {
            return HashCode.Combine(CursorPosition, Cost,
                Nodes.OrderBy(it => it.Key.X).ThenBy(it => it.Key.Y).Aggregate(0, (i, pair) => HashCode.Combine(i, pair.Value)),
                CursorTail.Aggregate(0, HashCode.Combine), GoalTail.Aggregate(0, HashCode.Combine));
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Day22Space other) return false;
            if (Cost != other.Cost) return false;
            if (!other.CursorPosition.Equals(CursorPosition)) return false;
            if (other.CursorTail.Count != CursorTail.Count) return false;
            if (CursorTail.Zip(other.CursorTail).Any(it => !it.First.Equals(it.Second))) return false;
            if (Nodes.Any(my => !other.Nodes[my.Key].Equals(my.Value))) return false;
            if (other.GoalTail.Count != GoalTail.Count) return false;
            if (GoalTail.Zip(other.GoalTail).Any(it => !it.First.Equals(it.Second))) return false;
            return true;
        }
    }

    public record Day22NodeContents(int Size, int Used);

    public static class Day22NodeContentsExtensions
    {
        public static int Avail(this Day22NodeContents self) => self.Size - self.Used;
    }

    public class Day22NodeRx
    {
        [RxFormat(Before = "/dev/grid/node-x")]
        public int X { get; set; }

        [RxFormat(Before = "-y")]
        public int Y { get; set; }

        [RxFormat(After = "T")]
        public int Size { get; set; }

        [RxFormat(After = "T")]
        public int Used { get; set; }

        [RxFormat(After = "T")]
        public int Avail { get; set; }

        [RxFormat(After = "%")]
        public int UsePercent { get; set; }
    }
}