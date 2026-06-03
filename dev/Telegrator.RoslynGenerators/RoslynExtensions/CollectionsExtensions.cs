/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Telegrator.RoslynGenerators.RoslynExtensions;

public static class CollectionsExtensions
{
    public static IEnumerable<TSource> Combine<TSource>(params IEnumerable<TSource>[] collections)
        => collections.SelectMany(x => x);

    public static IEnumerable<TSource> IntersectBy<TSource, TValue>(this IEnumerable<TSource> first, IEnumerable<TValue> second, Func<TSource, TValue> selector)
    {
        foreach (TSource item in first)
        {
            TValue value = selector(item);
            if (second.Contains(value))
                yield return item;
        }
    }

    public static IList<TValue> UnionAdd<TValue>(this IList<TValue> source, IEnumerable<TValue> toUnion, IEqualityComparer<TValue> comparer)
    {
        foreach (TValue toUnionValue in toUnion)
        {
            if (!source.Contains(toUnionValue, comparer))
                source.Add(toUnionValue);
        }

        return source;
    }

    public static void UnionAdd<TSource>(this ICollection<TSource> collection, IEnumerable<TSource> target)
    {
        foreach (TSource item in target)
        {
            if (!collection.Contains(item))
                collection.Add(item);
        }
    }

    public static void UnionAdd<TSource>(this SortedList<TSource, TSource> collection, IEnumerable<TSource> target)
    {
        foreach (TSource item in target)
        {
            if (!collection.Values.Contains(item))
                collection.Add(item, item);
        }
    }

    public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        int index = 0;
        foreach (T item in source)
        {
            if (predicate.Invoke(item))
                return index;

            index++;
        }

        return -1;
    }

    public static IEnumerable<T> Repeat<T>(this T item, int times)
        => Enumerable.Range(0, times).Select(_ => item);
}
