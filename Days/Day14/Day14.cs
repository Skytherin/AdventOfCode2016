using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AdventOfCode2016.Utils;
using FluentAssertions;

namespace AdventOfCode2016.Days.Day14
{
    public class Day14: IAdventOfCode
    {
        public void Run()
        {
            Do1("abc").Should().Be(22728);
            Do1("yjdafjpo").Should().Be(25427L);
            SuperCache.Clear();
            //Do1("abc", true).Should().Be(22551);
            //Do1("yjdafjpo", true).Should().Be(22045L);
        }

        private long Do1(string salt, bool keyStretching = false)
        {
            var cache = new List<string>();
            var keys = new List<string>();

            for (var i = 0; i < 100_000; i++)
            {
                var s = GetHash(i, salt, cache, keyStretching);
                if (TryGetRun(s, out var run))
                {
                    var needle = Enumerable.Repeat(run, 5).Join();
                    for (var j = 1; j <= 1000; j++)
                    {
                        var s2 = GetHash(i + j, salt, cache, keyStretching);
                        if (s2.Contains(needle))
                        {
                            keys.Add(s);
                            if (keys.Count == 64) return i;
                            break;
                        }
                    }
                }
            }

            throw new ApplicationException();
        }

        private readonly Dictionary<(string s, int i), string> SuperCache = new();

        private string GetHash(int n, string salt, List<string> cache, bool keyStretching)
        {
            if (n < cache.Count) return cache[n];
            cache.Count.Should().Be(n);
            var temp = SuperGetHash($"{salt}{n}", keyStretching ? 2016 : 0);
            cache.Add(temp);
            return temp;
        }

        private string SuperGetHash(string s, int i)
        {
            var key = (s, i);
            if (SuperCache.TryGetValue(key, out var c))
            {
                return c;
            }

            var temp = InternalGetHash(s);
            if (i == 0)
            {
                SuperCache[key] = temp;
                return temp;
            }

            var result = SuperGetHash(temp, i - 1);
            SuperCache[key] = result;
            return result;
        }

        private readonly Dictionary<string, string> HashCache = new();

        private string InternalGetHash(string s)
        {
            if (HashCache.TryGetValue(s, out var k)) return k;
            var md5 = MD5.HashData(Encoding.ASCII.GetBytes(s));
            var temp = BitConverter.ToString(md5).Replace("-", "").ToLower();
            HashCache[s] = temp;
            return temp;
        }

        private bool TryGetRun(string s, out char output)
        {
            var tail = s.First();
            var count = 1;
            foreach (var c in s.Skip(1))
            {
                if (c == tail)
                {
                    count += 1;
                    if (count == 3)
                    {
                        output = c;
                        return true;
                    }
                }
                else
                {
                    tail = c;
                    count = 1;
                }
            }

            output = default;
            return false;
        }
    }
}