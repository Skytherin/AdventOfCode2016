using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Days.Day12;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day25
{
    [UsedImplicitly]
    public class Day25: IAdventOfCode
    {
        public void Run()
        {
            var input = StructuredRx.ParseLines<AssembunnyRx>(this.Input()).Select(it => it.Which).ToList();
            var needle = -1L;
            for (var initialA = 0L; needle == -1L && initialA < 10000; initialA++)
            {
                Console.WriteLine(initialA);
                var abc = new AssembunnyComputer();
                abc.Registers['a'] = initialA;
                var closed = new HashSet<string>();
                var expected = 0;
                var n = 0;
                foreach (var output in abc.RunWithOutput(input))
                {
                    if (n++ >= 1000) break;
                    if (output != expected)
                    {
                        Console.WriteLine(n);
                        break;
                    }
                    if (!closed.Add($"{output},{abc.Registers['a']},{abc.Registers['b']},{abc.Registers['c']},{abc.Registers['d']}"))
                    {
                        needle = initialA;
                        break;
                    }

                    expected = expected == 0 ? 1 : 0;
                }
            }

            needle.Should().Be(-2);
        }
    }
}