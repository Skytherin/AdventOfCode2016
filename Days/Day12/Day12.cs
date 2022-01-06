using System.Collections.Generic;
using AdventOfCode2016.Utils;

namespace AdventOfCode2016.Days.Day12
{
    public class Day12: AdventOfCode<List<Assembunny>>
    {
        public override List<Assembunny> Parse(string input) => StructuredRx.ParseLines<Assembunny>(input);

        [TestCase(Input.Example, 42)]
        [TestCase(Input.Input, 318077L)]
        public override long Part1(List<Assembunny> input)
        {
            return Run(input);
        }

        [TestCase(Input.Input, 9227731L)]
        public override long Part2(List<Assembunny> input)
        {
            return Run(input, 1);
        }

        private static long Run(IList<Assembunny> input, int c = 0)
        {
            var reg = new Dictionary<string, int> { { "c", c } };

            var pc = 0;

            while (pc < input.Count)
            {
                var i = input[pc++];
                if (i.Copy is { } copy)
                {
                    reg[copy.DestinationReg] = copy.ConstValue ?? reg[copy.SourceRegValue!];
                }
                else if (i.Dec is { } dec)
                {
                    reg[dec.Reg] = reg.GetValueOrDefault(dec.Reg) - 1;
                }
                else if (i.Inc is { } inc)
                {
                    reg[inc.Reg] = reg.GetValueOrDefault(inc.Reg) + 1;
                }
                else if (i.Jnz is { } jnz)
                {
                    if ((jnz.ConstValue ?? reg.GetValueOrDefault(jnz.SourceRegValue!)) is not 0)
                    {
                        pc -= 1;
                        pc += jnz.JumpDistance;
                    }
                }
            }

            return reg["a"];
        }
    }

    public class Assembunny
    {
        [RxAlternate]
        public CopyInstruction Copy { get; set; }
        [RxAlternate]
        public IncrementInstruction Inc { get; set; }
        [RxAlternate]
        public DecrementInstruction Dec { get; set; }
        [RxAlternate]
        public JnzInstruction Jnz { get; set; }
    }

    public class JnzInstruction
    {
        [RxFormat(Before = "jnz"), RxAlternate]
        public int? ConstValue { get; set; }

        [RxFormat(Before = "jnz"), RxAlternate]
        public string? SourceRegValue { get; set; }

        public int JumpDistance { get; set; }
    }

    public class IncrementInstruction
    {
        [RxFormat(Before = "inc")]
        public string Reg { get; set; }
    }

    public class DecrementInstruction
    {
        [RxFormat(Before = "dec")]
        public string Reg { get; set; }
    }

    public class CopyInstruction
    {
        [RxFormat(Before="cpy"), RxAlternate]
        public int? ConstValue { get; set; }

        [RxFormat(Before = "cpy"), RxAlternate]
        public string? SourceRegValue { get; set; }

        public string DestinationReg { get; set; }
    }
}