using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day01
{
    [UsedImplicitly]
    public class Day01 : AdventOfCode<List<string>>
    {
        public override List<string> Parse(string input) => input.Split(",").Select(it => it.Trim()).ToList();

        [TestCase(Input.Input, 242)]
        public override long Part1(List<string> input)
        {
            Walk(Parse("R2,L3")).Last().ManhattanDistance().Should().Be(5);
            Walk(Parse("R2, R2, R2")).Last().ManhattanDistance().Should().Be(2);
            Walk(Parse("R5, L5, R5, R3")).Last().ManhattanDistance().Should().Be(12);
            return Walk(input).Last().ManhattanDistance();
        }

        [TestCase(Input.Input, 150)]
        public override long Part2(List<string> input)
        {
            var visited = new HashSet<Position> { Position.Zero };
            foreach (var location in Walk(input))
            {
                if (!visited.Add(location)) return location.ManhattanDistance();
            }

            throw new ApplicationException();
        }

        private IEnumerable<Position> Walk(List<string> instructions)
        {
            var position = Position.Zero;
            var vector = Vector.North;

            foreach (var instruction in instructions)
            {
                vector = instruction[0] == 'R' ? vector.RotateRight() : vector.RotateLeft();
                var d = Convert.ToInt32(instruction.Substring(1));
                for (var i = 1; i <= d; i++)
                {
                    position += vector;
                    yield return position;
                }
            }
        }
    }
}