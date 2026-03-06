using System.Collections.ObjectModel;
using System.Reflection;
using Telegrator.Core;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator
{
    /// <summary>
    /// Provides extension methods for working with collections.
    /// </summary>
    public static partial class ColletionsExtensions
    {
        /// <summary>
        /// Creates a <see cref="ReadOnlyDictionary{TKey, TValue}"/> from an <see cref="IEnumerable{TValue}"/>
        /// according to a specified key selector function.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector) where TKey : notnull
        {
            Dictionary<TKey, TValue> dictionary = source.ToDictionary(keySelector);
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Enumerates objects in a <paramref name="source"/> and executes an <paramref name="action"/> on each one
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<TValue> ForEach<TValue>(this IEnumerable<TValue> source, Action<TValue> action)
        {
            foreach (TValue value in source)
                action.Invoke(value);

            return source;
        }

        /// <summary>
        /// Sets the value of a key in a dictionary, or if the key does not exist, adds it
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, value);
        }

        /// <summary>
        /// Sets the value of a key in a dictionary, or if the key does not exist, adds its default value.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value, TValue defaultValue)
        {
            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, defaultValue);
        }

        /// <summary>
        /// Return the random object from <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TSource Random<TSource>(this IEnumerable<TSource> source)
            => source.Random(new Random());

        /// <summary>
        /// Return the random object from <paramref name="source"/>
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static TSource Random<TSource>(this IEnumerable<TSource> source, Random random)
            => source.ElementAt(random.Next(0, source.Count() - 1));

        /// <summary>
        /// Adds a range of elements to collection if they dont already exist using default equality comparer
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="list"></param>
        /// <param name="elements"></param>
        public static void UnionAdd<TSource>(this IList<TSource> list, params IEnumerable<TSource> elements)
        {
            foreach (TSource item in elements)
            {
                if (!list.Contains(item, EqualityComparer<TSource>.Default))
                    list.Add(item);
            }
        }

        /// <summary>
        /// Return index of first element that satisfies the condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the only element of a sequence, or a default value if the sequence is empty.
        /// This method returns default if there is more than one element in the sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T? SingleSafe<T>(this IEnumerable<T> source)
            => source.Count() == 1 ? source.ElementAt(0) : default;

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists.
        /// This method return default if more than one element satisfies the condition.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T? SingleSafe<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            source = source.Where(predicate);
            return source.Count() == 1 ? source.ElementAt(0) : default;
        }
    }

    /// <summary>
    /// Provides extension methods for reflection and type inspection.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        /// <summary>
        /// Checks if a type implements the <see cref="ICustomDescriptorsProvider"/> interface.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type implements ICustomDescriptorsProvider; otherwise, false.</returns>
        public static bool IsCustomDescriptorsProvider(this Type type)
            => type.GetInterface(nameof(ICustomDescriptorsProvider)) != null;

        /// <summary>
        /// Checks if <paramref name="type"/> is a <see cref="IFilter{T}"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsFilterType(this Type type)
            => type.IsAssignableToGenericType(typeof(IFilter<>));

        /// <summary>
        /// Checks if <paramref name="type"/> is a descendant of <see cref="UpdateHandlerBase"/> class
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsHandlerAbstract(this Type type)
            => type.IsAbstract && typeof(UpdateHandlerBase).IsAssignableFrom(type);

        /// <summary>
        /// Checks if <paramref name="type"/> is an implementation of <see cref="UpdateHandlerBase"/> class or its descendants
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsHandlerRealization(this Type type)
            => !type.IsAbstract && type != typeof(UpdateHandlerBase) && typeof(UpdateHandlerBase).IsAssignableFrom(type);

        /// <summary>
        /// Checks if <paramref name="type"/> has a parameterless constructor
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasParameterlessCtor(this Type type)
            => type.GetConstructors().Any(ctor => ctor.GetParameters().Length == 0);

        /// <summary>
        /// Checks is <paramref name="type"/> has public properties
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasPublicProperties(this Type type)
            => type.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(prop => prop.Name != "IsCollectible").Any();

        /// <summary>
        /// Determines whether an instance of a specified type can be assigned to an instance of the current type
        /// </summary>
        /// <param name="givenType"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType.GetInterfaces().Any(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == genericType))
                return true;

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            if (givenType.BaseType == null)
                return false;

            return givenType.BaseType.IsAssignableToGenericType(genericType);
        }
    }

    /// <summary>
    /// Provides extension methods for string manipulation.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Slices a <paramref name="source"/> string into a array of substrings of fixed <paramref name="length"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<string> SliceBy(this string source, int length)
        {
            for (int start = 0; start < source.Length; start += length + 1)
            {
                int tillEnd = source.Length - start;
                int toSlice = tillEnd < length + 1 ? tillEnd : length + 1;

                ReadOnlySpan<char> chunk = source.AsSpan().Slice(start, toSlice);
                yield return chunk.ToString();
            }
        }

        /// <summary>
        /// Return new string with first found letter set to upper case
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string FirstLetterToUpper(this string target)
        {
            char[] chars = target.ToCharArray();
            int index = chars.IndexOf(char.IsLetter);
            chars[index] = char.ToUpper(chars[index]);
            return new string(chars);
        }

        /// <summary>
        /// Return new string with first found letter set to lower case
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string FirstLetterToLower(this string target)
        {
            char[] chars = target.ToCharArray();
            int index = chars.IndexOf(char.IsLetter);
            chars[index] = char.ToLower(chars[index]);
            return new string(chars);
        }

        /// <summary>
        /// Checks if string contains a 'word'.
        /// 'Word' must be a separate member of the text, and not have any alphabetic characters next to it.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="word"></param>
        /// <param name="comparison"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static bool ContainsWord(this string source, string word, StringComparison comparison = StringComparison.InvariantCulture, int startIndex = 0)
        {
            int index = source.IndexOf(word, startIndex, comparison);
            if (index == -1)
                return false;

            if (index > 0)
            {
                char prev = source[index - 1];
                if (char.IsLetter(prev))
                    return false;
            }

            if (index + word.Length < source.Length)
            {
                char post = source[index + word.Length];
                if (char.IsLetter(post))
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Contains extension method for number types
    /// </summary>
    public static class NumbersExtensions
    {
        /// <summary>
        /// Check if int value has int flag using bit compare
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool HasFlag(this int value, int flag)
            => (value & flag) == flag;

        /// <summary>
        /// Check if int value has enum flag using bit compare
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool HasFlag<T>(this int value, T flag) where T : Enum
            => value.HasFlag(Convert.ToInt32(flag));
    }
}
