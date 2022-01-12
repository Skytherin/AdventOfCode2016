using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;

namespace AdventOfCode2016.Days.Day13
{
    public class Day13: IAdventOfCode
    {
        public void Run()
        {
            Do1(10, 7, 4).Should().Be(11);
            Do1(1364, 31, 39).Should().Be(86);

            Do2(1364).Should().Be(127);
        }

        private long Do2(int n)
        {
            var open = new Queue<(Position P, long Cost)>();
            open.Enqueue((new Position(1,1), 0));
            var closed = new Dictionary<Position, long>();
            while (open.TryDequeue(out var current))
            {
                if (closed.TryGetValue(current.P, out var other) && other <= current.Cost) continue;
                closed[current.P] = current.Cost;

                if (current.Cost == 50) continue;

                foreach (var neighbor in Neighbors(current.P, n))
                {
                    var totalCost = neighbor.Cost + current.Cost;
                    open.Enqueue((neighbor.Node, totalCost));
                }
            }

            return closed.LongCount();
        }

        private long Do1(int n, int targetX, int targetY)
        {
            var target = new Position(targetY, targetX);

            return SearchAlgorithm.AStarSearch(new Position(1, 1), new Position(targetY, targetX), p => Neighbors(p, n),
                p => p.ManhattanDistance(target)).Steps;
        }

        private long Fn(Position p, long value) => p.X * p.X + 3 * p.X + 2 * p.X * p.Y + p.Y + p.Y * p.Y + value;

        private IEnumerable<(long Cost, Position Node)> Neighbors(Position arg, long value)
        {
            foreach (var p in arg.Orthogonal().Where(p => p.Y >= 0 && p.X >= 0))
            {
                var b = CountBits(Fn(p, value));
                if (b % 2 == 0)
                {
                    yield return (1, p);
                }
            }
        }

        private long CountBits(long n)
        {
            var count = 0;
            while (n > 0)
            {
                if ((n & 1) == 1)
                {
                    count += 1;
                }
                
                n >>= 1;
            }

            return count;
        }
    }
}