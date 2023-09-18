using System;
using System.Collections.Generic;

namespace Fast.UAParser;

internal static class DictionaryExtensions
{
    public static TValue Find<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        if (dictionary == null)
            throw new ArgumentNullException(nameof(dictionary));
        TValue obj;
        return !dictionary.TryGetValue(key, out obj) ? default(TValue) : obj;
    }
}