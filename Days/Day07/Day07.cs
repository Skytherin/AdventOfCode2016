using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day07
{
    [UsedImplicitly]
    public class Day07 : AdventOfCode<List<string>>
    {
        public override List<string> Parse(string input)
        {
            return input.Lines();
        }

        [TestCase(Input.Input, 105)]
        public override long Part1(List<string> input)
        {
            SupportsTls("abba[mnop]qrst").Should().BeTrue();
            SupportsTls("abcd[bddb]xyyx").Should().BeFalse();
            SupportsTls("aaaa[qwer]tyui").Should().BeFalse();
            SupportsTls("ioxxoj[asdfgh]zxcvbn").Should().BeTrue();

            return input.Count(SupportsTls);
        }

        [TestCase(Input.Input, 258)]
        public override long Part2(List<string> input)
        {
            SupportsSsl("aba[bab]xyz").Should().BeTrue();
            SupportsSsl("xyx[xyx]xyx").Should().BeFalse();
            SupportsSsl("aaa[kek]eke").Should().BeTrue();
            SupportsSsl("zazbz[bzb]cdb").Should().BeTrue();

            return input.Count(SupportsSsl);
        }

        private bool SupportsSsl(string ipAddress)
        {
            var list = ipAddress.Split("[").SelectMany(it => it.Split("]")).ToList();

            var abaSequences = new List<string>();

            foreach (var item in list.WithIndices().Where(it => it.Index % 2 == 0).Select(it => it.Value))
            {
                var tail = item.Take(2).ToList();
                foreach (var c in item.Skip(2))
                {
                    tail.Add(c);
                    if (tail[0] == c && c != tail[1]) abaSequences.Add(tail.Join());
                    tail = tail.Skip(1).ToList();
                }
            }

            foreach (var item in list.WithIndices().Where(it => it.Index % 2 == 1).Select(it => it.Value))
            {
                foreach (var sequence in abaSequences)
                {
                    var bab = $"{sequence[1]}{sequence[0]}{sequence[1]}";
                    if (item.Contains(bab)) return true;
                }
            }

            return false;
        }

        private bool SupportsTls(string ipAddress)
        {
            var list = ipAddress.Split("[").SelectMany(it => it.Split("]")).ToList();

            var abbaSupernet = false;
            var abbaHypernet = false;

            foreach (var item in list.WithIndices())
            {
                if (ContainsAbba(item.Value))
                {
                    if (item.Index % 2 == 0)
                        abbaSupernet = true;
                    else
                        abbaHypernet = true;
                }
            }

            return abbaSupernet && !abbaHypernet;
        }

        private bool ContainsAbba(string s)
        {
            var tail = s.Take(3).ToList();
            foreach (var c in s.Skip(3))
            {
                if (tail[0] == c && tail[1] == tail[2] && c != tail[1])
                {
                    return true;
                }

                tail = tail.Skip(1).Append(c).ToList();
            }

            return false;
        }
    }
}