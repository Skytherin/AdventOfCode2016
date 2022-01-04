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
            Do1(Example).Should().Be(11);
            Do1(Input).Should().Be(47);
            Console.WriteLine("\n***");
            Do1(Input2).Should().Be(0);
        }

        private int Do1(Day11Data initial)
        {
            return SearchAlgorithm.BreadthFirstSearch(initial, Goal(initial), Neighbors, Valid).Cost;
        }

        private bool Valid(Day11Data input)
        {
            return true;
        }

        private static bool FloorIsValid(IReadOnlySet<Component> floor)
        {
            var generators = floor.Where(it => it.Type == ComponentType.Generator).ToList();
            if (!generators.Any()) return true;
            var chips = floor.Where(it => it.Type == ComponentType.Microchip);

            return chips.All(c => generators.Any(g => g.Element == c.Element));
        }

        private IEnumerable<Day11Data> Neighbors(Day11Data input)
        {
            var itemsOnFloor = input.Floors[input.Elevator];

            foreach (var choice in itemsOnFloor
                         .Select(item => new List<Component>{item})
                         .AppendAll(itemsOnFloor.Choose(2))
                     )
            {
                foreach (var newElevator in new []{input.Elevator-1, input.Elevator+1}.Where(floor => floor is >= 0 and <= 3))
                {
                    var floors = input.Floors.ToList();
                    floors[input.Elevator] = floors[input.Elevator].Except(choice).ToHashSet();
                    floors[newElevator] = floors[newElevator].Union(choice).ToHashSet();
                    if (FloorIsValid(floors[input.Elevator]) && FloorIsValid(floors[newElevator]))
                    {
                        yield return new(newElevator, floors);
                    }
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

        public Day11Data(int elevator, IEnumerable<IEnumerable<Component>> floors)
        {
            Elevator = elevator;
            Floors = floors.Select(it => it.ToHashSet()).ToList();
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
            return Floors.Zip(other.Floors).All(zipped => zipped.First.Union(zipped.Second).Count() == zipped.First.Count);
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