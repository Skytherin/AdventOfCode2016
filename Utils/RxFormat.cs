using System;

namespace AdventOfCode2016.Utils
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RxFormat : Attribute
    {
        public string? After { get; set; }

        public string? Before { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RxAlternate : Attribute
    {
        public bool Restart { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RxIgnore : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RxRepeat : Attribute
    {
        public readonly int Min;
        public readonly int Max;
        public readonly string Separator;

        public RxRepeat(int min = 0, int max = int.MaxValue, string separator = " ")
        {
            Separator = separator;
            Min = min;
            Max = max;
        }
    }
}