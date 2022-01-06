using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day08
{
    [UsedImplicitly]
    public class Day08 : AdventOfCode<List<IDay08Instruction>>
    {
        public override List<IDay08Instruction> Parse(string input)
        {
            return input.Lines().Select(it => 
                StructuredRx.ParseOrDefault<RectInstruction>(it) ??
                StructuredRx.ParseOrDefault<RotateRowInstruction>(it) as IDay08Instruction ??
                StructuredRx.ParseOrDefault<RotateColumnInstruction>(it)).ToList();
        }

        [TestCase(Input.Input, 116)]
        public override long Part1(List<IDay08Instruction> input)
        {
            var grid = Enumerable.Range(0, 6).Select(_ => Enumerable.Range(0, 50).Select(_ => false).ToReadOnlyList())
                .ToReadOnlyList();

            grid = input.Aggregate(grid, (accum, current) => current.Operate(accum));

            Console.WriteLine();
            for (var y = 0; y < 6; y++)
            {
                for (var x = 0; x < 50; x++)
                {
                    if (grid[y][x]) Console.Write("#");
                    else Console.Write(" ");
                }
                Console.WriteLine();
            }

            return grid.SelectMany(row => row).Count(it => it);
        }

        public override long Part2(List<IDay08Instruction> input)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IDay08Instruction
    {
        IReadOnlyList<IReadOnlyList<bool>> Operate(IReadOnlyList<IReadOnlyList<bool>> grid);
    }

    public class RectInstruction: IDay08Instruction
    {
        [RxFormat(Before = "rect", After = "x")]
        public int Columns { get; set; }
        public int Rows { get; set; }

        public IReadOnlyList<IReadOnlyList<bool>> Operate(IReadOnlyList<IReadOnlyList<bool>> grid)
        {
            return grid.WithIndices()
                .Select(row => row.Value.WithIndices()
                    .Select(col => row.Index < Rows && col.Index < Columns || col.Value).ToList())
                .ToList();
        }
    }

    public class RotateRowInstruction : IDay08Instruction
    {
        [RxFormat(Before = "rotate row y=", After = "by")]
        public int Row { get; set; }
        public int Magnitude { get; set; }

        public IReadOnlyList<IReadOnlyList<bool>> Operate(IReadOnlyList<IReadOnlyList<bool>> grid)
        {
            var result = grid.ToList();
            result[Row] = result[Row].RotateRight(Magnitude).ToList();
            return result;
        }
    }

    public class RotateColumnInstruction : IDay08Instruction
    {
        [RxFormat(Before = "rotate column x=", After = "by")]
        public int Column { get; set; }
        public int Magnitude { get; set; }

        public IReadOnlyList<IReadOnlyList<bool>> Operate(IReadOnlyList<IReadOnlyList<bool>> grid)
        {
            var flipped = grid.Flip().ToList();
            flipped[Column] = flipped[Column].RotateRight(Magnitude).ToList();
            return flipped.Flip().ToList();
        }
    }
}