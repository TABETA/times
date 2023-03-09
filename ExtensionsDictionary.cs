using System.Collections.Generic;

namespace RA
{
    public static class DictionaryExtensions
    {
        public static TV GetOrDefault<TK, TV>(this Dictionary<TK, TV> dic, TK key, TV defaultValue = default) => dic.TryGetValue(key, out var result) ? result : defaultValue;

    }
}
