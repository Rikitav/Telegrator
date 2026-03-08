using Telegram.Bot.Types;
using Telegrator.Core.Filters;
using Telegrator.Core.States;

namespace Telegrator.Filters
{
    /// <summary>
    /// Filters updates by comparing a resolved state key with a target key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key resolver used to get state key.</typeparam>
    /// <typeparam name="TValue">The type of the key used for state resolution.</typeparam>
    public class StateKeyFilter<TKey, TValue> : Filter<Update>
        where TKey : IStateKeyResolver, new()
        where TValue : IEquatable<TValue>
    {
        private readonly TValue? TargetKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateKeyFilter{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="targetKey">The target key to compare with.</param>
        public StateKeyFilter(TValue? targetKey)
        {
            TargetKey = targetKey;
        }

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<Update> context)
        {
            string? key = new TKey().ResolveKey(context.Input);
            if (key is null)
                return TargetKey is null;

            TValue? value = context.UpdateRouter.StateStorage.GetAsync<TValue>(key).Result;
            if (value is null)
                return TargetKey is null;

            if (TargetKey is null)
                return false;

            return TargetKey.Equals(value);
        }
    }
}
