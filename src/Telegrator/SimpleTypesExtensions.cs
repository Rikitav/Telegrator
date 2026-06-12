/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Telegrator.Core;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator;

/// <summary>
/// Provides extension methods for working with collections.
/// </summary>
public static partial class ColletionsExtensions
{
    private static readonly ThreadLocal<Random> _threadLocalRandom = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));
    private static Random SharedRandom => _threadLocalRandom.Value;

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
    /// Remove all <see langword="null"/> values and returns collection without nullable type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<T> Squeeze<T>(this IEnumerable<T?> source)
    {
        foreach (T? item in source)
        {
            if (item is not null)
                yield return item;
        }
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
    public static IDictionary<TKey, TValue> Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
    {
        source[key] = value;
        return source;
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
    public static IDictionary<TKey, TValue> Set<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value, TValue defaultValue)
    {
        if (source.ContainsKey(key))
            source[key] = value;
        else
            source.Add(key, defaultValue);

        return source;
    }

    /// <summary>
    /// Return the random object from <paramref name="source"/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static TSource Random<TSource>(this IEnumerable<TSource> source)
        => source.Random(SharedRandom);

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
    public static T? SingleOrNothing<T>(this IEnumerable<T> source)
    {
        if (source is IList<T> list)
            return list.Count == 1 ? list[0] : default;

        using IEnumerator<T> enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            return default;

        T result = enumerator.Current;
        if (enumerator.MoveNext())
            return default;

        return result;
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists.
    /// This method return default if more than one element satisfies the condition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T? SingleOrNothing<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        return source.Where(predicate).SingleOrNothing();
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
    public static bool IsCustomDescriptorsProvider([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type)
        => type.GetInterface(nameof(ICustomDescriptorsProvider)) != null;

    /// <summary>
    /// Checks if <paramref name="type"/> is a <see cref="IFilter{T}"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsFilterType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type)
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
    public static bool IsHandlerImplementation(this Type type)
        => !type.IsAbstract && type != typeof(UpdateHandlerBase) && typeof(UpdateHandlerBase).IsAssignableFrom(type);

    /// <summary>
    /// Checks if <paramref name="type"/> has a parameterless constructor
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HasParameterlessCtor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] this Type type)
        => type.GetConstructor(Type.EmptyTypes) != null;

    /// <summary>
    /// Checks is <paramref name="type"/> has public properties
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HasPublicProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] this Type type)
        => type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Any(prop => prop.Name != "IsCollectible");

    /// <summary>
    /// Determines whether an instance of a specified type can be assigned to an instance of the current type
    /// </summary>
    /// <param name="givenType"></param>
    /// <param name="genericType"></param>
    /// <returns></returns>
    public static bool IsAssignableToGenericType(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type givenType, Type genericType)
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
    /// Truncates the string to the specified maximum length, appending an ellipsis if truncated.
    /// </summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">The maximum length including the ellipsis.</param>
    /// <param name="ellipsis">The suffix to append when truncating.</param>
    /// <returns>The truncated string.</returns>
    public static string Truncate(this string value, int maxLength, string ellipsis = "…")
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        if (maxLength <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength));

        if (value.Length <= maxLength)
            return value;

        int take = Math.Max(0, maxLength - ellipsis.Length);
        return value[..take] + ellipsis;
    }

    /// <summary>
    /// Returns the first line of the string, or the whole string if it contains no newlines.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>The first line.</returns>
    public static string FirstLine(this string text)
    {
        if (text is null)
            throw new ArgumentNullException(nameof(text));

        int index = text.IndexOfAny(['\r', '\n']);
        return index < 0 ? text : text[..index];
    }

    /// <summary>
    /// Slices a <paramref name="source"/> string into a array of substrings of fixed <paramref name="length"/>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static IEnumerable<string> SliceBy(this string source, int length)
    {
        for (int start = 0; start < source.Length; start += length)
        {
            int tillEnd = source.Length - start;
            int toSlice = tillEnd < length ? tillEnd : length;

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
