using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Days.Day12;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day23
{
    [UsedImplicitly]
    public class Day23: IAdventOfCode
    {
        public IReadOnlyList<IAssembunnyInstruction> Parse(string s) => StructuredRx.ParseLines<AssembunnyRx>(s).Select(it => it.Which).ToList();

        public void Run()
        {
            var input = Parse(this.Input());
            var abc = new AssembunnyComputer();
            abc.Registers['a'] = 7;
            abc.Run(input);
            abc.Registers['a'].Should().Be(12330);
            Console.WriteLine("\n******************");
            abc = new AssembunnyComputer();
            abc.Registers['a'] = 12;
            abc.Run(input);
            abc.Registers['a'].Should().Be(479008890L);
        }
    }
}