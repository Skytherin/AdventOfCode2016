using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;

namespace AdventOfCode2016.Days.Day21
{
    public class Day21: IAdventOfCode
    {
        public List<Day21Input> Parse(string input) => StructuredRx.ParseLines<Day21Input>(input);

        public void Run()
        {
            Do1(Parse(this.Example()), "abcde").Should().Be("decab");
            Do1(Parse(this.Input()), "abcdefgh").Should().Be("gbhafcde");

            Do2(Parse(this.Example()), "decab").Should().Be("abcde");
            Do2(Parse(this.Input()), "gbhafcde").Should().Be("abcdefgh");

            Do2(Parse(this.Input()), "fbgdceah").Should().Be("bcfaegdh");
        }

        public string Do1(List<Day21Input> input, string start)
        {
            return input.Aggregate(start.ToList(), (current, item) => item.Operate(current)).Join();
        }

        public string Do2(List<Day21Input> input, string start)
        {
            return input.Select(it => it).Reverse().Aggregate(start.ToList(), (current, item) => item.Reverse(current)).Join();
        }
    }

    public class Day21Input
    {
        [RxAlternate]
        public SwapPosition? SwapPosition { get; set; }
        [RxAlternate]
        public SwapLetter? SwapLetter { get; set; }
        [RxAlternate]
        public Rotate? Rotate { get; set; }
        [RxAlternate]
        public RotateFromLetter? RotateFromLetter { get; set; }
        [RxAlternate]
        public ReverseOp? ReverseSub { get; set; }
        [RxAlternate]
        public Move? Move { get; set; }

        public List<char> Operate(List<char> current)
        {
            var temp = SwapPosition?.Operate(current) ??
                   SwapLetter?.Operate(current) ??
                   Rotate?.Operate(current) ??
                   RotateFromLetter?.Operate(current) ??
                   ReverseSub?.Operate(current) ??
                   Move?.Operate(current) ?? throw new ApplicationException();

            return temp;
        }

        public List<char> Reverse(List<char> current)
        {
            var temp = SwapPosition?.Reverse(current) ??
                       SwapLetter?.Reverse(current) ??
                       Rotate?.Reverse(current) ??
                       RotateFromLetter?.Reverse(current) ??
                       ReverseSub?.Reverse(current) ??
                       Move?.Reverse(current) ?? throw new ApplicationException();

            return temp;
        }
    }

    public class SwapPosition
    {
        [RxFormat(Before = "swap position")]
        public int X { get; set; }

        [RxFormat(Before = "with position")]
        public int Y { get; set; }

        public List<char> Operate(List<char> current)
        {
            (current[Y], current[X]) = (current[X], current[Y]);
            return current;
        }

        public List<char> Reverse(List<char> current) => Operate(current);
    }

    public class SwapLetter
    {
        [RxFormat(Before = "swap letter")]
        public char X { get; set; }

        [RxFormat(Before = "with letter")]
        public char Y { get; set; }

        public List<char> Operate(List<char> current)
        {
            var x = current.IndexOf(X);
            var y = current.IndexOf(Y);
            (current[y], current[x]) = (current[x], current[y]);
            return current;
        }

        public List<char> Reverse(List<char> current) => Operate(current);
    }

    public class Rotate
    {
        [RxFormat(Before = "rotate")]
        public Direction RotateDirection { get; set; }

        public int Steps { get; set; }

        public string Unused { get; set; } = "";

        public enum Direction
        {
            Left,
            Right
        };

        public List<char> Operate(List<char> current)
        {
            if (RotateDirection == Direction.Left)
            {
                return current.RotateLeft(Steps).ToList();
            }

            return current.RotateRight(Steps).ToList();
        }

        public List<char> Reverse(List<char> current) => new Rotate
        {
            Steps = Steps,
            RotateDirection = RotateDirection == Direction.Left ? Direction.Right : Direction.Left
        }.Operate(current);
    }

    public class RotateFromLetter
    {
        [RxFormat(Before = "rotate based on position of letter")]
        public char X { get; set; }

        public List<char> Operate(List<char> current)
        {
            var index = current.IndexOf(X);
            var n = 1 + index + (index >= 4 ? 1 : 0);
            return current.RotateRight(n).ToList();
        }

        public List<char> Reverse(List<char> current)
        {
            for (var i = 0; i < current.Count; i++)
            {
                var temp = current.RotateLeft(i).ToList();
                if (current.Join() == Operate(temp).Join())
                {
                    return temp;
                }
            }

            throw new ApplicationException();
        }
    }


    public class ReverseOp
    {
        [RxFormat(Before = "reverse positions")]
        public int X { get; set; }

        [RxFormat(Before = "through")]
        public int Y { get; set; }

        public List<char> Operate(List<char> current)
        {
            return current.Take(X).AppendAll(current.Skip(X).Take(Y - X + 1).Reverse())
                .AppendAll(current.Skip(Y + 1))
                .ToList();
        }

        public List<char> Reverse(List<char> current) => Operate(current);
    }

    public class Move
    {
        [RxFormat(Before = "move position")]
        public int X { get; set; }

        [RxFormat(Before = "to position")]
        public int Y { get; set; }

        public List<char> Operate(List<char> current)
        {
            var c = current[X];
            current.RemoveAt(X);
            current.Insert(Y, c);
            return current;
        }

        public List<char> Reverse(List<char> current)
        {
            var c = current[Y];
            current.RemoveAt(Y);
            current.Insert(X, c);
            return current;
        }
    }
}