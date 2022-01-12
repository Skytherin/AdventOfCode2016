using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AdventOfCode2016.Utils;
using FluentAssertions;

namespace AdventOfCode2016.Days.Day17
{
    public class Day17: IAdventOfCode
    {
        public void Run()
        {
            Do1("ihgpwlah").Should().Be("DDRRRD");
            Do1("kglvqrro").Should().Be("DDUDRLRRUDRD");
            Do1("ulqzkmiv").Should().Be("DRURDRUDDLLDLUURRDULRLDUUDDDRR");
            Do1("hhhxzeay").Should().Be("DDRUDLRRRD");

            Do2("ihgpwlah").Should().Be(370);
            Do2("kglvqrro").Should().Be(492);
            Do2("ulqzkmiv").Should().Be(830);
            Do2("hhhxzeay").Should().Be(398);
        }

        private string Do1(string seed)
        {
            var result = SearchAlgorithm.AStarSearchAll(new Day17Node(new Position(0, 0), ""),
                node => node.Position.Equals(new Position(3, 3)),
                node => Neighbors(node, seed), node => node.Position.ManhattanDistance(new Position(3, 3)));

            return result.First().Node.Path;
        }

        private long Do2(string seed)
        {
            var result = SearchAlgorithm.AStarSearchAll(new Day17Node(new Position(0, 0), ""),
                node => node.Position.Equals(new Position(3, 3)),
                node => Neighbors(node, seed), node => node.Position.ManhattanDistance(new Position(3, 3)));

            return result.Max(it => it.Steps);
        }

        private IEnumerable<(long Cost, Day17Node Node)> Neighbors(Day17Node node, string seed)
        {
            var md5 = MD5.HashData(Encoding.ASCII.GetBytes($"{seed}{node.Path}")).Take(4).ToArray();
            var temp = BitConverter.ToString(md5).Replace("-", "").ToLower();

            foreach (var direction in new[]
                     {
                         ("U", node.Position.North, temp[0]),
                         ("D", node.Position.South, temp[1]),
                         ("L", node.Position.West, temp[2]),
                         ("R", node.Position.East, temp[3])
                     }.Where(it => it.Item2.X is >= 0 and <= 3 && 
                                   it.Item2.Y is >= 0 and <= 3 &&
                                   IsOpen(it.Item3)))
            {
                yield return (1, new Day17Node(direction.Item2, node.Path + direction.Item1));
            }
        }

        private bool IsOpen(char c)
        {
            return c is >= 'b' and <= 'f';
        }
    }

    public record Day17Node(Position Position, string Path);
}