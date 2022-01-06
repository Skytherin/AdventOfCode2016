using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;

namespace AdventOfCode2016.Days.Day15
{
    public class Day15: AdventOfCode<List<Day15Data>>
    {
        public override List<Day15Data> Parse(string input) => StructuredRx.ParseLines<Day15Data>(input);
        
        [TestCase(Input.Example, 5)]
        [TestCase(Input.Input, 400589L)]
        public override long Part1(List<Day15Data> input)
        {
            var sentinel = input.Select(it => (long)it.Positions).Product();
            for (var t = 0L; t < sentinel; t++)
            {
                if (input.All(item => (item.StartingPosition + item.DiskNumber + t) % item.Positions == 0))
                {
                    return t;
                }
            }

            throw new ApplicationException();
        }

        [TestCase(Input.Input, 3045959L)]
        public override long Part2(List<Day15Data> input)
        {
            return Part1(input.Append(new Day15Data
            {
                DiskNumber = input.Count + 1,
                Positions = 11,
                StartingPosition = 0
            }).ToList());
        }
    }

    public class Day15Data
    {
        [RxFormat(Before = "Disc #")]
        public int DiskNumber { get; set; }
        [RxFormat(Before = "has ", After = "positions; at time=0, it is at position")]
        public int Positions { get; set; }
        [RxFormat(After = ".")]
        public int StartingPosition { get; set; }
    }
}