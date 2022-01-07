using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;

namespace AdventOfCode2016.Days.Day18
{
    public class Day18: IAdventOfCode
    {
        public void Run()
        {
            Do1(".^^.^.^^^^", 10).Should().Be(38);
            Do1(this.Input().Lines().First(), 40).Should().Be(1913);
            Do1(this.Input().Lines().First(), 400000).Should().Be(19993564);
        }

        private long Do1(string first, int rows)
        {
            var result = new List<string> { first };
            while (result.Count < rows)
            {
                var last = result[^1];
                var current = "";
                for (var i = 0; i < last.Length; i++)
                {
                    current += IsTrapped(last, i) ? "^" : ".";
                }
                result.Add(current);
            }

            return result.SelectMany(it => it).Count(it => it == '.');
        }

        private bool IsTrapped(string last, int i)
        {
            var leftIsTrap = i > 0 && IsTrapped(last[i - 1]);
            var rightIsTrap = i < last.Length - 1 && IsTrapped(last[i + 1]);

            return (leftIsTrap && !rightIsTrap) || (!leftIsTrap && rightIsTrap);
        }

        private bool IsTrapped(char c) => c == '^';
    }
}