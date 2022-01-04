using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day11
{
    [UsedImplicitly]
    public class Day11: IAdventOfCode
    {
        public void Run()
        {
            Console.WriteLine();
            Do1(Example).Should().Be(11);
            Console.WriteLine("**");
            Do1(Input).Should().Be(47);
            Console.WriteLine("***");
            Do1(Input2).Should().Be(71);
        }

        private int Do1(Day11Data initial)
        {
            return SearchAlgorithm.AStarSearch(initial, Goal(initial), Neighbors, PriorityFunction).Cost;
        }

        private int PriorityFunction(Day11Data input)
        {
            var cost = 0;
            foreach (var i in new[]{2,1,0})
            {
                cost += Moves(input.Floors[i].Count) * (3 - i);
            }

            return cost;
        }

        private int Moves(int numberOfItems)
        {
            if (numberOfItems == 0) return 0;
            if (numberOfItems <= 2) return 1;
            return numberOfItems * 2 - 3;
        }

        private static bool FloorIsValid(IReadOnlySet<Component> floor)
        {
            var generators = floor.Where(it => it.Type == ComponentType.Generator).ToList();
            if (!generators.Any()) return true;
            var chips = floor.Where(it => it.Type == ComponentType.Microchip);

            return chips.All(c => generators.Any(g => g.Element == c.Element));
        }

        private IEnumerable<(int Cost, Day11Data Node)> Neighbors(Day11Data input)
        {
            var floors = new List<int>();
            if (input.Elevator < 3) floors.Add(input.Elevator + 1);
            for (var i = 0; i < input.Elevator; i++)
            {
                if (input.Floors[i].Any())
                {
                    floors.Add(input.Elevator - 1);
                    break;
                }
            }

            if (!floors.Any()) yield break;

            var itemsOnFloor = input.Floors[input.Elevator];
            foreach (var choice in itemsOnFloor
                         .Choose(1)
                         .AppendAll(itemsOnFloor.Choose(2))
                     )
            {
                var thisFloor = input.Floors[input.Elevator].Except(choice).ToHashSet();
                if (!FloorIsValid(thisFloor)) continue;
                foreach (var newElevator in floors)
                {
                    var newFloors = input.Floors.ToList();
                    newFloors[input.Elevator] = thisFloor;
                    newFloors[newElevator] = newFloors[newElevator].AppendAll(choice).ToHashSet();
                    if (!FloorIsValid(newFloors[newElevator])) continue;
                    yield return (1, new(newElevator, newFloors));
                }
            }
        }

        private Day11Data Example => new(0, new List<HashSet<Component>>
        {
            new(){ new("hydrogen", ComponentType.Microchip), new("lithium", ComponentType.Microchip) },
            new(){ new("hydrogen", ComponentType.Generator) },
            new(){ new("lithium", ComponentType.Generator) },
            new()
        });

        private Day11Data Input => new(0, new List<HashSet<Component>>
        {
            new()
            {
                new("polonium", ComponentType.Generator), new("thulium", ComponentType.Generator),
                new("thulium", ComponentType.Microchip), new("promethium", ComponentType.Generator),
                new("ruthenium", ComponentType.Generator), new("ruthenium", ComponentType.Microchip),
                new("cobalt", ComponentType.Generator), new("cobalt", ComponentType.Microchip)
            },
            new(){ new("polonium", ComponentType.Microchip), new("promethium", ComponentType.Microchip) },
            new(),
            new()
        });

        private Day11Data Input2 => new(0, new List<HashSet<Component>>
        {
            new()
            {
                new("polonium", ComponentType.Generator), new("thulium", ComponentType.Generator),
                new("thulium", ComponentType.Microchip), new("promethium", ComponentType.Generator),
                new("ruthenium", ComponentType.Generator), new("ruthenium", ComponentType.Microchip),
                new("cobalt", ComponentType.Generator), new("cobalt", ComponentType.Microchip),
                new("elerium", ComponentType.Microchip), new("elerium", ComponentType.Generator),
                new("dilithium", ComponentType.Microchip), new("dilithium", ComponentType.Generator),
            },
            new(){ new("polonium", ComponentType.Microchip), new("promethium", ComponentType.Microchip) },
            new(),
            new()
        });

        private Day11Data Goal(Day11Data initial)
        {
            var fourthFloor = initial.Floors.SelectMany(it => it).ToHashSet();
            return new Day11Data(3, new List<HashSet<Component>> { new(), new(), new(), fourthFloor });
        }
    }

    public class Day11Data
    {
        public readonly int Elevator;
        public readonly IReadOnlyList<IReadOnlySet<Component>> Floors;
        private readonly int MyHashCode;
        private readonly string ReducedState;

        public Day11Data(int elevator, IEnumerable<IEnumerable<Component>> floors)
        {
            Elevator = elevator;
            Floors = floors.Select(it => it.ToHashSet()).ToList();

            ReducedState = floors.Select(floor => $"{floor.Count(it => it.)}{}").Join(";");

            MyHashCode = HashCode.Combine(Elevator, Floors.Aggregate(0, (current, value) =>
                HashCode.Combine(current, value.OrderBy(it => it.Element).ThenBy(it => it.Type)
                    .Aggregate(0, HashCode.Combine))
            ));
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Day11Data other) return false;
            if (other.Elevator != Elevator) return false;
            if (Floors.Count != other.Floors.Count) return false;
            return Floors.Zip(other.Floors).All(zipped => zipped.First.Count == zipped.Second.Count && 
                                                          zipped.First.Union(zipped.Second).Count() == zipped.First.Count);
        }

        public override int GetHashCode() => MyHashCode;
    }

    public record Component(string Element, ComponentType Type);

    public enum ComponentType
    {
        Generator,
        Microchip
    }
}