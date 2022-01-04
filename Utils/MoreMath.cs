namespace AdventOfCode2016.Utils
{
    public static class MoreMath
    {
        public static long Sign(long a) => a switch
        {
            < 0 => -1,
            > 0 => 1,
            _ => 0
        };
        public static long Abs(long a) => a < 0 ? -a : a;

        public static int RoundUp(int numerator, int denominator) =>
            numerator / denominator + ((numerator % denominator) > 0 ? 1 : 0);
    }
}