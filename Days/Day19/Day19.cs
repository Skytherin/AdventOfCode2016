using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;

namespace AdventOfCode2016.Days.Day19
{
    public class Day19: IAdventOfCode
    {
        public void Run()
        {
            Do1(5).Should().Be(3);
            Do1(3_014_603).Should().Be(1834903);

            Do2(5).Should().Be(2);
            Do2(3_014_603).Should().Be(1420280);
        }

        private int Do2(int elfCount)
        {
            var elves = new LinkedList<int>(Enumerable.Range(1, elfCount));

            var current = elves.First!;
            for (var i = 0; i < elves.Count / 2; i++)
            {
                current = current!.Next!;
            }

            while (elves.Count > 1)
            {
                var next1 = current.Roll();
                var next2 = next1.Roll();
                elves.Remove(current);
                current = elves.Count % 2 == 1 ? next1 : next2;
            }

            return elves.Single();
        }

        private int Do1(int elfCount)
        {
            var elves = new LinkedList<int>(Enumerable.Range(1, elfCount));

            var current = elves.First!.Next!;

            while (elves.Count > 1)
            {
                var next2 = current.Roll().Roll();
                elves.Remove(current);
                current = next2;
            }

            return elves.Single();
        }
    }
}