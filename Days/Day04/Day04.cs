using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2016.Utils;
using FluentAssertions;
using JetBrains.Annotations;

namespace AdventOfCode2016.Days.Day04
{
    [UsedImplicitly]
    public class Day04 : AdventOfCode<List<RoomData>>
    {
        public override List<RoomData> Parse(string input) => StructuredRx.ParseLines<RoomData>(input);

        [TestCase(Input.Input, 185371)]
        public override long Part1(List<RoomData> input)
        {
            IsRealRoom(StructuredRx.Parse<RoomData>("aaaaa-bbb-z-y-x-123[abxyz]")).Should().BeTrue();
            IsRealRoom(StructuredRx.Parse<RoomData>("a-b-c-d-e-f-g-h-987[abcde]")).Should().BeTrue();
            IsRealRoom(StructuredRx.Parse<RoomData>("not-a-real-room-404[oarel]")).Should().BeTrue();
            IsRealRoom(StructuredRx.Parse<RoomData>("totally-real-room-200[decoy]")).Should().BeFalse();
            return input.Where(IsRealRoom).Sum(it => it.SectorId);
        }

        [TestCase(Input.Input, 984)]
        public override long Part2(List<RoomData> input)
        {
            Rotate('a', 1).Should().Be('b');
            Rotate('a', 25).Should().Be('z');
            Rotate('a', 26).Should().Be('a');
            "qzmt".Select(c => Rotate(c, 343)).Join().Should().Be("very");

            return input
                .Where(IsRealRoom)
                .Where(room => room.EncryptedName.Select(word => word.Select(c => Rotate(c, room.SectorId)).Join()).Join(" ") == "northpole object storage")
                .Single()
                .SectorId;
        }

        public char Rotate(char c, int amount)
        {
            return (char)('a' + (c - 'a' + amount) % 26);
        }

        private bool IsRealRoom(RoomData arg)
        {
            var top5 = arg.EncryptedName.Join("").GroupBy(it => it).OrderByDescending(it => it.Count()).ThenBy(it => it.Key).Select(it => it.Key).Take(5).Join();
            return top5 == arg.Checksum;
        }
    }

    public class RoomData
    {
        [RxRepeat(1, separator:"-")]
        [RxFormat(After = "-")]
        public List<string> EncryptedName { get; set; }
        public int SectorId { get; set; }

        [RxFormat(Before = "[", After = "]")]
        public string Checksum { get; set; }
    }
}