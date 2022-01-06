using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2016.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day03
{
    [UsedImplicitly]
    public class Day03 : AdventOfCode<List<Day03Triangle>>
    {
        public override List<Day03Triangle> Parse(string input) => StructuredRx.ParseLines<Day03Triangle>(input);

        [TestCase(Input.Input, 862)]
        public override long Part1(List<Day03Triangle> input)
        {
            return input.Count(i => i.IsValid);
        }

        [TestCase(Input.Input, 1577)]
        public override long Part2(List<Day03Triangle> input)
        {
            var lines = File.ReadAllText($"Days/Day03/Input.txt")
                .Lines()
                .Select(line => line.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                     .Select(it => Convert.ToInt32(it)).ToList())
                .ToList();
            return
                lines.Flip().SelectMany(it => it)
                    .InGroupsOf(3)
                    .Select(it => new Day03Triangle
                    {
                        L1 = it[0], 
                        L2 = it[1], 
                        L3 = it[2]
                    })
                    .Count(it => it.IsValid);
        }
    }

    public class Day03Triangle
    {
        public bool IsValid => (L1 + L2 > L3) && (L1 + L3 > L2) && (L3 + L2 > L1);
        public int L1 { get; set; }
        public int L2 { get; set; }
        public int L3 { get; set; }
    }
}