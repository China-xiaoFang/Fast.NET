using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fast.UAParser;

internal static class RegexBinderBuilder
{
    public static Func<Match, IEnumerator<int>, TResult> SelectMany<T1, T2, TResult>(
        this Func<Match, IEnumerator<int>, T1> binder, Func<T1, Func<Match, IEnumerator<int>, T2>> continuation,
        Func<T1, T2, TResult> projection)
    {
        return (m, num) =>
        {
            var obj1 = binder(m, num);
            var obj2 = continuation(obj1)(m, num);
            return projection(obj1, obj2);
        };
    }
}