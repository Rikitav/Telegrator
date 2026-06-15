using Telegram.Bot.Types;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Filter annotation that matches callback queries by their <see cref="CallbackQuery.Data"/> payload.
/// Supports exact matching, prefix matching, and case-insensitive comparison.
/// </summary>
public class CallbackDataFilterAttribute : FilterAnnotation<CallbackQuery>
{
    /// <summary>
    /// Gets the exact callback data value to match.
    /// </summary>
    public string? Data { get; }

    /// <summary>
    /// Gets or sets the callback data prefix to match.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// Gets whether the comparison is case-insensitive.
    /// </summary>
    public bool IgnoreCase { get; }

    /// <summary>
    /// Initializes a new instance that matches callback data exactly.
    /// </summary>
    /// <param name="data">The exact callback data to match.</param>
    /// <param name="ignoreCase">Whether to ignore case when comparing.</param>
    public CallbackDataFilterAttribute(string data, bool ignoreCase = true)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentException("Callback data cannot be null or empty.", nameof(data));

        Data = data;
        IgnoreCase = ignoreCase;
    }

    private CallbackDataFilterAttribute(string data, string prefix, bool ignoreCase = true)
    {
        Data = data;
        Prefix = prefix;
        IgnoreCase = ignoreCase;
    }

    /// <summary>
    /// Creates a filter that matches callback queries whose data starts with the specified prefix.
    /// </summary>
    /// <param name="prefix">The prefix to match.</param>
    /// <param name="ignoreCase">Whether to ignore case when comparing.</param>
    /// <returns>A new <see cref="CallbackDataFilterAttribute"/> configured for prefix matching.</returns>
    public static CallbackDataFilterAttribute WithPrefix(string prefix, bool ignoreCase = true)
    {
        if (string.IsNullOrEmpty(prefix))
            throw new ArgumentException("Prefix cannot be null or empty.", nameof(prefix));

        return new CallbackDataFilterAttribute(string.Empty, prefix, ignoreCase);
    }

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
    {
        if (context.Input.Data is not { Length: > 0 } data)
            return false;

        StringComparison comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        if (Prefix is not null)
            return data.StartsWith(Prefix, comparison);

        if (Data is not null)
            return data.Equals(Data, comparison);

        return false;
    }
}
