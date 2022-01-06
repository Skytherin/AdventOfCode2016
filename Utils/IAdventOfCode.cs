using System;
using JetBrains.Annotations;

namespace AdventOfCode2016.Utils
{
    public interface IAdventOfCode
    {
        void Run();
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseAttribute: Attribute
    {
        public Input Input { get; }
        public long Expected { get; }

        public TestCaseAttribute(Input input, long expected)
        {
            Input = input;
            Expected = expected;
        }
    }

    public enum Input
    {
        Example,
        Input
    }
}