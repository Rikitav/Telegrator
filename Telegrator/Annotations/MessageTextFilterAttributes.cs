﻿using Telegrator.Filters;

namespace Telegrator.Annotations
{
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
}
