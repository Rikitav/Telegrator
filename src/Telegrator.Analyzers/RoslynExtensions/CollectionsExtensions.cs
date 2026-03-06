namespace Telegrator.Analyzers.RoslynExtensions
{
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

        public static IList<TValue> UnionAdd<TValue>(this IList<TValue> source, IEnumerable<TValue> toUnion)
        {
            foreach (TValue toUnionValue in toUnion)
            {
                if (!source.Contains(toUnionValue, EqualityComparer<TValue>.Default))
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
}
