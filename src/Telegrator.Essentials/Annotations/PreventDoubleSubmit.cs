using System.Collections.Concurrent;
using Telegram.Bot.Types;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Filter annotation that prevents the same callback query from being processed twice
/// within a specified timeout window. Useful for guarding against rapid double-taps
/// on inline keyboard buttons.
/// </summary>
public class PreventDoubleSubmit : FilterAnnotation<CallbackQuery>
{
    private readonly ConcurrentDictionary<int, DateTime> _ids = [];

    /// <summary>
    /// Gets or sets the debounce window in seconds. Callback queries from the same user
    /// with the same query id received inside this window are ignored.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 5;

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
    {
        int hash = HashCodeCombine(context.Input.From.Id.GetHashCode(), context.Input.Id.GetHashCode());
        DateTime now = DateTime.UtcNow;

        if (_ids.TryGetValue(hash, out DateTime last))
        {
            if ((now - last).TotalSeconds < TimeoutSeconds)
                return false;

            _ids[hash] = now;
        }
        else
        {
            _ids.TryAdd(hash, now);
        }

        return true;
    }

    private static int HashCodeCombine(int first, int second)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + first.GetHashCode();
            hash = hash * 31 + second.GetHashCode();
            return hash;
        }
    }
}
