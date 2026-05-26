using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering callback-based updates.
/// Supports various message types including regular messages, edited messages, channel posts, and business messages.
/// </summary>
/// <param name="filters">The filters to apply to messages</param>
public abstract class CallbackQueryAttribute(params IFilter<CallbackQuery>[] filters) : UpdateFilterAttribute<CallbackQuery>(filters)
{
    /// <summary>
    /// Gets the allowed update types that this filter can process.
    /// </summary>
    public override UpdateType[] AllowedTypes => [UpdateType.CallbackQuery];

    /// <summary>
    /// Extracts the message from various types of updates.
    /// </summary>
    /// <param name="update">The Telegram update</param>
    /// <returns>The message from the update, or null if not present</returns>
    public override CallbackQuery? GetFilterringTarget(Update update)
        => update.CallbackQuery;
}

/// <summary>
/// Attribute for filtering <see cref="CallbackQuery"/>'s data
/// </summary>
/// <param name="data"></param>
public class CallbackDataAttribute(string data)
    : CallbackQueryAttribute(new CallbackDataFilter(data))
{ }

/// <summary>
/// Attribute for filtering <see cref="CallbackQuery"/>'s data that contains specific string
/// </summary>
public class CallbackDataContainsAttribute(string data)
    : CallbackQueryAttribute(new CallbackDataContainsFilter(data))
{ }

/// <summary>
/// Attribute for filtering <see cref="CallbackQuery"/>'s data that starts with specific string
/// </summary>
public class CallbackDataStartsWithAttribute(string data)
    : CallbackQueryAttribute(new CallbackDataStartsWithFilter(data))
{ }

/// <summary>
/// Attribute for filtering <see cref="CallbackQuery"/>'s data that ends with specific string
/// </summary>
public class CallbackDataEndsWithAttribute(string data)
    : CallbackQueryAttribute(new CallbackDataEndsWithFilter(data))
{ }

/// <summary>
/// Attribute for filtering <see cref="CallbackQuery"/>'s data with Regex
/// </summary>
public class CallbackDataRegexAttribute(string pattern)
    : CallbackQueryAttribute(new CallbackRegexFilter(pattern))
{ }

/// <summary>
/// Attribute to check if <see cref="CallbackQuery"/> belongs to a specific message by its ID
/// </summary>
public class CallbackInlineIdAttribute(string inlineMessageId)
    : CallbackQueryAttribute(new CallbackInlineIdFilter(inlineMessageId))
{ }
