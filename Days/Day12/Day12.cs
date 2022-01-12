using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day12
{
    [UsedImplicitly]
    public class Day12: AdventOfCode<IReadOnlyList<IAssembunnyInstruction>>
    {
        public override IReadOnlyList<IAssembunnyInstruction> Parse(string input) =>
            StructuredRx.ParseLines<AssembunnyRx>(input).Select(it => it.Which).ToList();

        [TestCase(Input.Example, 42)]
        [TestCase(Input.Input, 318077L)]
        public override long Part1(IReadOnlyList<IAssembunnyInstruction> input)
        {
            return Run(input);
        }

        [TestCase(Input.Input, 9227731L)]
        public override long Part2(IReadOnlyList<IAssembunnyInstruction> input)
        {
            return Run(input, 1);
        }

        private static long Run(IReadOnlyList<IAssembunnyInstruction> input, int c = 0)
        {
            var abc = new AssembunnyComputer();
            abc.Registers['c'] = c;
            abc.Run(input);
            return abc.Registers['a'];
        }
    }

    public class AssembunnyRx
    {
        [RxAlternate]
        public CopyInstruction? Copy { get; set; }
        [RxAlternate]
        public IncrementInstruction? Inc { get; set; }
        [RxAlternate]
        public DecrementInstruction? Dec { get; set; }
        [RxAlternate]
        public JnzInstruction? Jnz { get; set; }
        [RxAlternate]
        public ToggleInstruction? Tgl { get; set; }
        [RxAlternate]
        public OutInstruction? Out { get; set; }

        public IAssembunnyInstruction Which => (Copy as IAssembunnyInstruction
                                                 ?? Inc ?? Dec ?? Jnz ?? Tgl as IAssembunnyInstruction ?? Out)!;
    }

    public class OutInstruction : IAssembunnyInstruction
    {
        [RxFormat(Before = "out"), RxAlternate]
        public char Reg { get; set; }

        public int Operate(Dictionary<char, long> registers, List<IAssembunnyInstruction> instructions, int pc)
        {
            throw new NotImplementedException();
        }

        public IAssembunnyInstruction Toggle()
        {
            throw new NotImplementedException();
        }
    }

    public class AssembunnyComputer
    {
        public readonly Dictionary<char, long> Registers = new()
        {
            { 'a', 0 },
            { 'b', 0 },
            { 'c', 0 },
            { 'd', 0 },
        };

        public void Run(IReadOnlyList<IAssembunnyInstruction> input)
        {
            var _ = RunWithOutput(input).ToList();
        }

        public IEnumerable<long> RunWithOutput(IReadOnlyList<IAssembunnyInstruction> input)
        {
            var pc = 0;
            var copy = input.ToList();
            while (pc < copy.Count)
            {
                if (DetectVirtualInstructions(copy, pc) is { } t)
                {
                    pc = t;
                }
                else if (copy[pc] is OutInstruction instr)
                {
                    yield return Registers[instr.Reg];
                    pc += 1;
                }
                else
                {
                    pc = copy[pc].Operate(Registers, copy, pc);
                }
            }
        }

        private int? DetectVirtualInstructions(
            IReadOnlyList<IAssembunnyInstruction> instructions, int pc)
        {
            return DetectMultiplication(instructions, pc) ?? DetectIncrement(instructions, pc);
        }

        private int? DetectIncrement(IReadOnlyList<IAssembunnyInstruction> instructions, int pc)
        {
            if (pc + 2 < instructions.Count &&
                instructions[pc] is IIncrementInstruction incr3 &&
                instructions[pc + 1] is IIncrementInstruction incr4 &&
                instructions[pc + 2] is JnzInstruction jnz &&
                (jnz.SourceRegValue == incr3.Reg || jnz.SourceRegValue == incr4.Reg)
                && jnz.JumpDistance == -2)
            {
                var otherReg = Registers[jnz.SourceRegValue!.Value];
                if (jnz.SourceRegValue == incr3.Reg)
                {
                    Registers[incr4.Reg] += incr4.Step * MoreMath.Abs(otherReg);
                }
                else
                {
                    Registers[incr3.Reg] += incr3.Step * MoreMath.Abs(otherReg);
                }

                Registers[jnz.SourceRegValue!.Value] = 0;
                return pc + 3;
            }

            return null;
        }

        private int? DetectMultiplication(IReadOnlyList<IAssembunnyInstruction> instructions, int pc)
        {
            if ((pc + 5) < instructions.Count &&
                instructions[pc] is CopyInstruction cpy &&
                instructions[pc + 1] is IIncrementInstruction incr1 &&
                instructions[pc + 2] is IIncrementInstruction incr2 &&
                instructions[pc + 3] is JnzInstruction jnz2 &&
                jnz2.SourceRegValue == incr2.Reg &&
                jnz2.JumpDistance == -2 &&
                instructions[pc + 4] is DecrementInstruction dec &&
                instructions[pc + 5] is JnzInstruction jnz1 &&
                jnz1.SourceRegValue == dec.Reg &&
                jnz1.JumpDistance == -5)
            {
                var r1 = cpy.ConstValue ?? Registers[cpy.SourceRegValue!.Value];
                var r2 = Registers[dec.Reg];

                if (incr2.Step > 0)
                {
                    r1 = r1 > 0 ? 1 : Math.Abs(r1);
                }
                else
                {
                    r1 = r1 < 0 ? 1 : r1;
                }

                if (dec.Step > 0)
                {
                    r2 = r2 > 0 ? 1 : Math.Abs(r2);
                }
                else
                {
                    r2 = r2 < 0 ? 1 : r2;
                }

                Registers[incr1.Reg] += incr1.Step * r1 * r2;
                Registers[jnz1.SourceRegValue!.Value] = 0;
                Registers[jnz2.SourceRegValue!.Value] = 0;
                return pc + 6;
            }

            return null;
        }
    }

    public interface IIncrementInstruction
    {
        public int Step { get; }
        public char Reg { get; }
    }

    public interface IAssembunnyInstruction
    {
        int Operate(Dictionary<char, long> registers, List<IAssembunnyInstruction> instructions, int pc);
        IAssembunnyInstruction Toggle();
    }

    public class ToggleInstruction: IAssembunnyInstruction
    {
        [RxFormat(Before = "tgl")]
        public char Reg { get; set; }

        public int Operate(Dictionary<char, long> registers, List<IAssembunnyInstruction> instructions, int pc)
        {
            if (pc + registers[Reg] < instructions.Count)
            {
                instructions[pc + (int)registers[Reg]] = instructions[pc + (int)registers[Reg]].Toggle();
            }
            
            return pc + 1;
        }

        public IAssembunnyInstruction Toggle()
        {
            return new IncrementInstruction
            {
                Reg = Reg
            };
        }
    }

    public class JnzInstruction: IAssembunnyInstruction
    {
        [RxFormat(Before = "jnz"), RxAlternate]
        public int? ConstValue { get; set; }

        [RxFormat(Before = "jnz"), RxAlternate]
        public char? SourceRegValue { get; set; }

        [RxAlternate(Restart = true)]
        public int? JumpDistance { get; set; }

        [RxAlternate]
        public char? JumpByReg { get; set; }

        public int Operate(Dictionary<char, long> registers, List<IAssembunnyInstruction> instructions, int pc)
        {
            if ((ConstValue ?? registers[SourceRegValue!.Value]) is not 0)
            {
                var distance = JumpDistance ?? registers[JumpByReg!.Value];
                return pc + (int)distance;
            }

            return pc + 1;
        }

        public IAssembunnyInstruction Toggle()
        {
            return new CopyInstruction
            {
                SourceRegValue = SourceRegValue,
                ConstValue = ConstValue,
                DestinationReg = JumpByReg!.Value
            };
        }
    }

    public class IncrementInstruction: IAssembunnyInstruction, IIncrementInstruction
    {
        public int Step => 1;

        [RxFormat(Before = "inc")]
        public char Reg { get; set; }

        public int Operate(Dictionary<char, long> registers, List<IAssembunnyInstruction> instructions, int pc)
        {
            registers[Reg] = registers.GetValueOrDefault(Reg) + 1;
            return pc + 1;
        }

        public IAssembunnyInstruction Toggle()
        {
            return new DecrementInstruction
            {
                Reg = Reg
            };
        }
    }

    public class DecrementInstruction: IAssembunnyInstruction, IIncrementInstruction
    {
        public int Step => -1;

        [RxFormat(Before = "dec")]
        public char Reg { get; set; }

        public int Operate(Dictionary<char, long> registers, List<IAssembunnyInstruction> instructions, int pc)
        {
            registers[Reg] = registers.GetValueOrDefault(Reg) - 1;
            return pc + 1;
        }

        public IAssembunnyInstruction Toggle()
        {
            return new IncrementInstruction()
            {
                Reg = Reg
            };
        }
    }

    public class CopyInstruction: IAssembunnyInstruction
    {
        [RxFormat(Before="cpy"), RxAlternate]
        public int? ConstValue { get; set; }

        [RxFormat(Before = "cpy"), RxAlternate]
        public char? SourceRegValue { get; set; }

        public char DestinationReg { get; set; }
        public int Operate(Dictionary<char, long> registers, List<IAssembunnyInstruction> instructions, int pc)
        {
            registers[DestinationReg] = ConstValue ?? registers[SourceRegValue!.Value];
            return pc + 1;
        }

        public IAssembunnyInstruction Toggle()
        {
            return new JnzInstruction
            {
                ConstValue = ConstValue,
                SourceRegValue = SourceRegValue,
                JumpDistance = 0,
                JumpByReg = DestinationReg
            };
        }
    }
}