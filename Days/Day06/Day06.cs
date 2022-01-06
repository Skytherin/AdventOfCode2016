using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day06
{
    [UsedImplicitly]
    public class Day06 : AdventOfCode<List<string>>
    {
        public override List<string> Parse(string input)
        {
            return input.Lines();
        }

        [TestCase(Input.Example, 0)]
        [TestCase(Input.Input, 0)]
        public override long Part1(List<string> input)
        {
            var result = input.Flip().Select(column => column.GroupBy(it => it).MaxBy(it => it.Count()).Key).Join();
            if (TestCase.Input == Input.Example) result.Should().Be("easter");
            else result.Should().Be("usccerug");
            return 0;
        }

        [TestCase(Input.Example, 0)]
        [TestCase(Input.Input, 0)]
        public override long Part2(List<string> input)
        {
            var result = input.Flip().Select(column => column.GroupBy(it => it).MinBy(it => it.Count()).Key).Join();
            if (TestCase.Input == Input.Example) result.Should().Be("advent");
            else result.Should().Be("cnvvtafc");
            return 0;
        }
    }
}