using System.Collections.Generic;

namespace AdventOfCode2016.Utils
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key) where TValue : new()
        {
            if (self.TryGetValue(key, out var temp)) return temp;
            temp = new TValue();
            self[key] = temp;
            return temp;
        }
    }
}