using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day02
{
    [UsedImplicitly]
    public class Day02 : AdventOfCode<List<string>>
    {
        public override List<string> Parse(string input) => input.Lines();

        [TestCase(Input.Example, 1985)]
        [TestCase(Input.Input, 24862)]
        public override long Part1(List<string> input)
        {
            Console.WriteLine();
            var code = new List<int>();
            var keypad = new Dictionary<Position, int>
            {
                { new Position(0, 0), 1 }, { new Position(0, 1), 2 }, { new Position(0, 2), 3 },
                { new Position(1, 0), 4 }, { new Position(1, 1), 5 }, { new Position(1, 2), 6 },
                { new Position(2, 0), 7 }, { new Position(2, 1), 8 }, { new Position(2, 2), 9 },
            };
            var position = new Position(1, 1);
            foreach (var line in input)
            {
                position = Walk(line, position, keypad.Keys.ToHashSet()).Last();
                code.Add(keypad[position]);
            }

            return code.Aggregate((accum, value) => accum * 10 + value);
        }

        [TestCase(Input.Input, 0)]
        public override long Part2(List<string> input)
        {
            var code = new List<char>();
            var keypad = new Dictionary<Position, char>
            {
                                                                          { new Position(0, 2), '1' },
                                             { new Position(1, 1), '2' }, { new Position(1, 2), '3' }, { new Position(1, 3), '4' },
                { new Position(2, 0), '5' }, { new Position(2, 1), '6' }, { new Position(2, 2), '7' }, { new Position(2, 3), '8' }, { new Position(2, 4), '9' },
                                             { new Position(3, 1), 'A' }, { new Position(3, 2), 'B' }, { new Position(3, 3), 'C' },
                                                                          { new Position(4, 2), 'D' },
            };
            var position = new Position(2, 0);
            foreach (var line in input)
            {
                position = Walk(line, position, keypad.Keys.ToHashSet()).Last();
                code.Add(keypad[position]);
            }

            code.Join().Should().Be("46C91");
            return 0;
        }

        private IEnumerable<Position> Walk(string instructions, Position position, HashSet<Position> validPositions)
        {
            foreach (var instruction in instructions)
            {
                var vector = instruction switch
                {
                    'U' => Vector.North,
                    'D' => Vector.South,
                    'L' => Vector.West,
                    'R' => Vector.East,
                };
                if (validPositions.Contains(position + vector))
                {
                    position += vector;
                    yield return position;
                }
            }
        }
    }
}