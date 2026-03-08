using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.States;
using Telegrator.Filters;

namespace Telegrator.Annotations;

public class StateAttribute<TKey, TValue>(TValue? value) : UpdateFilterAttribute<Update>(new StateKeyFilter<TKey, TValue>(value))
    where TKey : IStateKeyResolver, new()
    where TValue : IEquatable<TValue>
{
    public TValue? Value => value;

    public override UpdateType[] AllowedTypes => Update.AllTypes;

    public override Update? GetFilterringTarget(Update update)
    {
        return update;
    }
}
