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

using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Attribute for filtering messages where the text starts with the specified content.
/// </summary>
/// <param name="content">The string that the message text should start with</param>
/// <param name="comparison">The string comparison type</param>
public class TextStartsWithAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture)
    : MessageFilterAttribute(new TextStartsWithFilter(content, comparison))
{ }

/// <summary>
/// Attribute for filtering messages where the text ends with the specified content.
/// </summary>
/// <param name="content">The string that the message text should end with</param>
/// <param name="comparison">The string comparison type</param>
public class TextEndsWithAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture)
    : MessageFilterAttribute(new TextEndsWithFilter(content, comparison))
{ }

/// <summary>
/// Attribute for filtering messages where the text contains the specified content.
/// </summary>
/// <param name="content">The string that the message text should contain</param>
/// <param name="comparison">The string comparison type</param>
public class TextContainsAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture)
    : MessageFilterAttribute(new TextContainsFilter(content, comparison))
{ }

/// <summary>
/// Attribute for filtering messages where the text equals the specified content.
/// </summary>
/// <param name="content">The string that the message text should equal</param>
/// <param name="comparison">The string comparison type</param>
public class TextEqualsAttribute(string content, StringComparison comparison = StringComparison.InvariantCulture)
    : MessageFilterAttribute(new TextEqualsFilter(content, comparison))
{ }

/// <summary>
/// Attribute for filtering messages that contain any non-empty text.
/// </summary>
public class HasTextAttribute()
    : MessageFilterAttribute(new TextNotNullOrEmptyFilter())
{ }

/// <summary>
/// Attribute for filtering messages where the text contains a 'word'.
/// 'Word' must be a separate member of the text, and not have any alphabetic characters next to it.
/// </summary>
/// <param name="word"></param>
/// <param name="comparison"></param>
/// <param name="startIndex"></param>
public class TextContainsWordAttribute(string word, StringComparison comparison = StringComparison.InvariantCulture, int startIndex = 0)
    : MessageFilterAttribute(new TextContainsWordFilter(word, comparison, startIndex))
{ }
