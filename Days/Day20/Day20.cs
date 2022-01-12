using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day20
{
    [UsedImplicitly]
    public class Day20: AdventOfCode<List<Range>>
    {
        public override List<Range> Parse(string input) => StructuredRx.ParseLines<Range>(input);

        [TestCase(Input.Input, 31053880L)]
        public override long Part1(List<Range> input)
        {
            Do1(Parse(@"5-8
0-2
4-7")).Should().Be(3);
            return Do1(input);
        }

        [TestCase(Input.Input, 117)]
        public override long Part2(List<Range> input)
        {
            return Do2(input);
        }

        private long Do1(List<Range> input)
        {
            var allowed = new List<Range> { new() { Min = 0, Max = 4_294_967_295L } };
            return input.Aggregate(allowed,
                    (current, range) => current.SelectMany(it => it.Except(range)).ToList())
                .OrderBy(it => it.Min)
                .First()
                .Min;
        }

        private long Do2(List<Range> input)
        {
            var allowed = new List<Range> { new() { Min = 0, Max = 4_294_967_295L } };
            return input.Aggregate(allowed,
                    (current, range) => current.SelectMany(it => it.Except(range)).ToList())
                .Sum(it => it.Max - it.Min + 1);
        }
    }

    public class Range
    {
        public long Min { get; set; }

        [RxFormat(Before = "-")]
        public long Max { get; set; }
    }

    public static class RangeExtensions
    {
        public static IEnumerable<Range> Except(this Range self, Range other)
        {
            if (self.Max < other.Min || self.Min > other.Max)
            {
                yield return self;
                yield break;
            }

            if (self.Min < other.Min)
            {
                yield return new Range { Min = self.Min, Max = other.Min - 1 };
            }

            if (self.Max > other.Max)
            {
                yield return new Range { Min = other.Max + 1, Max = self.Max };
            }
        }
    }
}