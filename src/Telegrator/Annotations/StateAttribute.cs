using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.States;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Attribute for filtering updates where resolved state matches target value.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <param name="value"></param>
public class StateAttribute<TKey, TValue>(TValue? value) : UpdateFilterAttribute<Update>(new StateKeyFilter<TKey, TValue>(value))
    where TKey : IStateKeyResolver, new()
    where TValue : IEquatable<TValue>
{
    /// <summary>
    /// The targetting state value.
    /// </summary>
    public TValue? Value => value;

    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => Update.AllTypes;

    /// <inheritdoc/>
    public override Update? GetFilterringTarget(Update update)
    {
        return update;
    }
}
