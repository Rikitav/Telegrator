using Telegram.Bot.Types;
using Telegrator.StateKeeping.Components;

namespace Telegrator.StateKeeping
{
    /// <summary>
    /// Resolves sender ID from Telegram updates for state management purposes.
    /// Extracts the sender identifier from various types of updates to provide a consistent key for state operations.
    /// </summary>
    public class SenderIdResolver : IStateKeyResolver<long>
    {
        /// <summary>
        /// Resolves the sender ID from a Telegram update.
        /// </summary>
        /// <param name="keySource">The Telegram update to extract the sender ID from.</param>
        /// <returns>The sender ID as a long value.</returns>
        /// <exception cref="ArgumentException">Thrown when the update does not contain a valid sender ID.</exception>
        public long ResolveKey(Update keySource)
            => keySource.GetSenderId() ?? throw new ArgumentException("Cannot resolve SenderID for this Update");
    }
}
