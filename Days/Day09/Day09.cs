using System;
using System.Collections.Generic;
using AdventOfCode2016.Utils;
using FluentAssertions;

namespace AdventOfCode2016.Days.Day09
{
    public class Day09: AdventOfCode<string>
    {
        public override string Parse(string input) => input;

        [TestCase(Input.Input, 107035L)]
        public override long Part1(string input)
        {
            Do1("ADVENT").Should().Be(6);
            Do1("A(1x5)BC").Should().Be(7);
            Do1("(3x3)XYZ").Should().Be(9);
            Do1("A(2x2)BCD(2x2)EFG").Should().Be(11);
            Do1("(6x1)(1x3)A").Should().Be(6);
            Do1("X(8x2)(3x3)ABCY").Should().Be(18);
            return Do1(input);
        }

        [TestCase(Input.Input, 11451628995L)]
        public override long Part2(string input)
        {
            Do2("(3x3)XYZ").Should().Be(9);
            Do2("X(8x2)(3x3)ABCY").Should().Be("XABCABCABCABCABCABCY".Length);
            Do2("(27x12)(20x12)(13x14)(7x10)(1x12)A").Should().Be(241920);
            Do2("(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN").Should().Be(445);
            return Do2(input);
        }

        private long Do1(IEnumerable<char> input, bool recursive = false)
        {
            var buffer = new List<char>();
            var state = State.Default;
            var count = 0L;
            var markerLength = 0L;
            var markerRepeat = 0L;
            foreach (var c in input)
            {
                switch (state)
                {
                    case State.Default:
                        if (c == '(')
                        {
                            buffer.Clear();
                            markerLength = 0;
                            markerRepeat = 0;
                            state = State.MarkerLength;
                        }
                        else
                        {
                            count += 1;
                        }

                        break;
                    case State.MarkerLength:
                        if (c == 'x')
                        {
                            state = State.MarkerRepeat;
                        }
                        else
                        {
                            markerLength = markerLength * 10 + Convert.ToInt64($"{c}");
                        }

                        break;
                    case State.MarkerRepeat:
                        if (c == ')')
                        {
                            state = State.ReadingRepeatedRegion;
                        }
                        else
                        {
                            markerRepeat = markerRepeat * 10 + Convert.ToInt64($"{c}");
                        }

                        break;
                    case State.ReadingRepeatedRegion:
                        buffer.Add(c);
                        if (--markerLength <= 0)
                        {
                            count += recursive 
                                ? markerRepeat * Do1(buffer, true) 
                                : buffer.Count * markerRepeat;
                            state = State.Default;
                        }
                        break;
                    default:
                        throw new ApplicationException();
                }
            }

            return count;
        }

        private long Do2(string input) => Do1(input, true);


        private enum State
        {
            Default,
            MarkerLength,
            MarkerRepeat,
            ReadingRepeatedRegion
        }
    }
}