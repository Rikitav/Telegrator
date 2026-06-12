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

using System.Text.RegularExpressions;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Attribute for filtering messages where a command has arguments count >= <paramref name="count"/>.
/// </summary>
/// <param name="count"></param>
public class ArgumentCountAttribute(int count)
    : MessageFilterAttribute(new ArgumentCountFilter(count))
{ }

/// <summary>
/// Attribute for filtering messages where a command argument starts with the specified content.
/// </summary>
/// <param name="content">The content that the command argument should start with.</param>
/// <param name="comparison">The string comparison type to use for the check.</param>
/// <param name="index">The index of the argument to check (0-based).</param>
public class ArgumentStartsWithAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0)
    : MessageFilterAttribute(new ArgumentStartsWithFilter(content, comparison, index))
{ }

/// <summary>
/// Attribute for filtering messages where a command argument ends with the specified content.
/// </summary>
/// <param name="content">The content that the command argument should end with.</param>
/// <param name="comparison">The string comparison type to use for the check.</param>
/// <param name="index">The index of the argument to check (0-based).</param>
public class ArgumentEndsWithAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0)
    : MessageFilterAttribute(new ArgumentEndsWithFilter(content, comparison, index))
{ }

/// <summary>
/// Attribute for filtering messages where a command argument contains the specified content.
/// </summary>
/// <param name="content">The content that the command argument should contain.</param>
/// <param name="comparison">The string comparison type to use for the check.</param>
/// <param name="index">The index of the argument to check (0-based).</param>
public class ArgumentContainsAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0)
    : MessageFilterAttribute(new ArgumentContainsFilter(content, comparison, index))
{ }

/// <summary>
/// Attribute for filtering messages where a command argument equals the specified content.
/// </summary>
/// <param name="content">The content that the command argument should equal.</param>
/// <param name="comparison">The string comparison type to use for the check.</param>
/// <param name="index">The index of the argument to check (0-based).</param>
public class ArgumentEqualsAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0)
    : MessageFilterAttribute(new ArgumentEqualsFilter(content, comparison, index))
{ }

/// <summary>
/// Attribute for filtering messages where a command argument matches a regular expression pattern.
/// </summary>
/// <param name="pattern">The regular expression pattern to match against the command argument.</param>
/// <param name="options">The regex options to use for the pattern matching.</param>
/// <param name="index">The index of the argument to check (0-based).</param>
public class ArgumentRegexAttribute(string pattern, RegexOptions options = RegexOptions.None, int index = 0)
    : MessageFilterAttribute(new ArgumentRegexFilter(pattern, options, index: index))
{ }
