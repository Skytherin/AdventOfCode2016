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
            input.Reverse();

            foreach (var startTime in StartTimes(input.First()))
            {
                var allGood = true;
                foreach (var item in input.Skip(1))
                {
                    var startTime2 = StartTimes(item).First();
                    if (startTime2 > startTime)
                    {
                        allGood = false;
                        break;
                    }

                    if (startTime2 == startTime) continue;

                    if ((startTime - startTime2) % item.Positions == 0) continue;

                    allGood = false;
                    break;
                }

                if (allGood) return startTime;
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

        private IEnumerable<long> StartTimes(Day15Data item)
        {
            var first = item.Positions - (item.StartingPosition + item.DiskNumber) % item.Positions;
            if (first == item.Positions) first = 0;
            yield return first;
            while (true)
            {
                first += item.Positions;
                yield return first;
            }
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