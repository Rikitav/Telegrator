using Telegram.Bot.Types;

namespace Telegrator.StateKeeping.Components
{
    /// <summary>
    /// Defines a resolver for extracting a key from an update for state keeping purposes.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public interface IStateKeyResolver<TKey> where TKey : notnull
    {
        /// <summary>
        /// Resolves a key from the specified <see cref="Update"/>.
        /// </summary>
        /// <param name="keySource">The update to resolve the key from.</param>
        /// <returns>The resolved key.</returns>
        public TKey ResolveKey(Update keySource);
    }
}
