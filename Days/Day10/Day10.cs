using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day10
{
    [UsedImplicitly]
    public class Day10: AdventOfCode<List<IDay10Instruction>>
    {
        public override List<IDay10Instruction> Parse(string input)
        {
            return input.Lines()
                .Select(line =>
                {
                    var match1 = StructuredRx.ParseOrDefault<SrxInput>(line);
                    if (match1 is not null)
                    {
                        return new ReceiveFromInputInstruction(match1.Value, match1.Bot) as IDay10Instruction;
                    }

                    var match2 = StructuredRx.ParseOrDefault<SrxGive>(line);
                    if (match2 is not null)
                    {
                        return new BotCompareInstruction(
                            match2.Bot,
                            match2.LowDestination,
                            match2.LowNumber,
                            match2.HighDestination,
                            match2.HighNumber
                        );
                    }

                    throw new ApplicationException();
                }).ToList();
        }

        [TestCase(Input.Input, 118L)]
        public override long Part1(List<IDay10Instruction> input)
        {
            foreach (var (botNetwork, _) in Run(input))
            {
                var needle = botNetwork.Where(bot => bot.Value.Holds(17, 61)).Select(it => it.Key).ToList();
                if (needle.Any())
                {
                    return needle.First();
                }
            }

            throw new ApplicationException();
        }

        [TestCase(Input.Input, 143153L)]
        public override long Part2(List<IDay10Instruction> input)
        {
            var (_, outputNetwork) = Run(input).Last();

            return outputNetwork[0].First() * outputNetwork[1].First() * outputNetwork[2].First();
        }

        private IEnumerable<(Dictionary<int, Bot>, Dictionary<int, List<int>>)> Run(List<IDay10Instruction> input)
        {
            var botNetwork = new Dictionary<int, Bot>();
            var outputNetwork = new Dictionary<int, List<int>>();

            var changes = true;
            var closed = new HashSet<ReceiveFromInputInstruction>();
            while (changes)
            {
                changes = false;
                foreach (var instruction in input.Except(closed))
                {
                    var changed = instruction.Operate(botNetwork, outputNetwork);
                    if (changed)
                    {
                        if (instruction is ReceiveFromInputInstruction t) closed.Add(t);
                        yield return (botNetwork, outputNetwork);
                    }
                    changes = changes || changed;
                }
            }
        }
    }

    public class Bot
    {
        public int? Low { get; private set; }
        public int? High { get; private set; }

        public bool Full => Low is not null && High is not null;

        public bool Holds(int item1, int item2) => (Low == item1 && High == item2) || (Low == item2 && High == item1);

        public void Receive(int item)
        {
            if (Low == null && High == null)
            {
                Low = item;
            }
            else if (High == null)
            {
                if (item > Low)
                {
                    High = item;
                }
                else
                {
                    High = Low;
                    Low = item;
                }
            }
            else
            {
                throw new ApplicationException();
            }
        }

        public int GiveLow()
        {
            var item = (int)Low;
            Low = null;
            return item;
        }

        public int GiveHigh()
        {
            var item = (int)High;
            High = null;
            return item;
        }
    }

    public interface IDay10Instruction
    {
        public bool Operate(Dictionary<int, Bot> network, Dictionary<int, List<int>> outputNetwork);
    }

    public class ReceiveFromInputInstruction : IDay10Instruction
    {
        private readonly int BotNumber;
        private readonly int Value;

        public ReceiveFromInputInstruction(int value, int botNumber)
        {
            Value = value;
            BotNumber = botNumber;
        }

        public bool Operate(Dictionary<int, Bot> network, Dictionary<int, List<int>> outputNetwork)
        {
            if (!network.TryGetValue(BotNumber, out var bot))
            {
                bot = new Bot();
                network[BotNumber] = bot;
            }
            bot.Receive(Value);
            return true;
        }
    }

    public class BotCompareInstruction : IDay10Instruction
    {
        private readonly int BotNumber;
        private readonly Destination LowDestination;
        private readonly int LowDestinationNumber;
        private readonly Destination HighDestination;
        private readonly int HighDestinationNumber;

        public BotCompareInstruction(int botNumber, Destination lowDestination, int lowDestinationNumber, Destination highDestination, int highDestinationNumber)
        {
            BotNumber = botNumber;
            LowDestination = lowDestination;
            LowDestinationNumber = lowDestinationNumber;
            HighDestination = highDestination;
            HighDestinationNumber = highDestinationNumber;
        }

        public bool Operate(Dictionary<int, Bot> network, Dictionary<int, List<int>> outputNetwork)
        {
            if (!network.TryGetValue(BotNumber, out var bot)) return false;
            if (!bot.Full) return false;
            PassTo(network, outputNetwork, LowDestination, LowDestinationNumber, bot.GiveLow());
            PassTo(network, outputNetwork, HighDestination, HighDestinationNumber, bot.GiveHigh());
            return true;
        }

        private void PassTo(Dictionary<int, Bot> network, Dictionary<int, List<int>> outputNetwork, Destination destination, int destinationNumber,
            int item)
        {
            if (destination == Destination.Output)
            {
                outputNetwork.GetOrCreate(destinationNumber).Add(item);
            }
            else
            {
                network.GetOrCreate(destinationNumber).Receive(item);
            }
        }
    }

    public class SrxInput
    {
        [RxFormat(Before = "value", After = "goes to bot")]
        public int Value { get; set; }
        public int Bot { get; set; }
    }

    public class SrxGive
    {
        [RxFormat(Before = "bot", After = "gives low to")]
        public int Bot { get; set; }
        public Destination LowDestination { get; set; }
        public int LowNumber { get; set; }

        [RxFormat(Before = "and high to")]
        public Destination HighDestination { get; set; }
        public int HighNumber { get; set; }
    }

    public enum Destination
    {
        Output,
        Bot
    }
}