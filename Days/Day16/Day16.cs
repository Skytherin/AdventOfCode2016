using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;

namespace AdventOfCode2016.Days.Day16
{
    public class Day16: IAdventOfCode
    {
        public void Run()
        {
            Do1(20, "10000").Should().Be("01100");
            Do1(272, "01000100010010111").Should().Be("10010010110011010");
            Do1(35_651_584, "01000100010010111").Should().Be("01010100101011100");
        }

        private string Do1(int size, string seed)
        {
            while (seed.Length < size)
            {
                seed = seed + "0" + seed.Select(it => it == '1' ? '0' : '1').Reverse().Join();
            }

            var checksum = Checksum(seed.Substring(0, size));
            while (checksum.Length % 2 == 0) checksum = Checksum(checksum);
            return checksum;
        }

        private readonly Dictionary<string, string> Cache = new()
        {
            { "00", "1"},
            { "01", "0" },
            { "10", "0" },
            { "11", "1" },
        };

        private string Checksum(string s)
        {
            if (Cache.TryGetValue(s, out var c)) return c;

            if (s.Length % 4 == 0)
            {
                var x2 = Checksum(s.Substring(0, s.Length / 2)) + Checksum(s.Substring(s.Length / 2));
                Cache[s] = x2;
                return x2;
            }

            var x = Cache[s.Substring(0, 2)] + Checksum(s.Substring(2));
            Cache[s] = x;
            return x;
        }
    }
}