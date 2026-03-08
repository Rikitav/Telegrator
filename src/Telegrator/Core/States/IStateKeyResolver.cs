using Telegram.Bot.Types;

namespace Telegrator.Core.States;

/// <summary>
/// Defines a resolver for extracting a key from an update for state keeping purposes.
/// </summary>
public interface IStateKeyResolver
{
    /// <summary>
    /// Resolves a key from the specified <see cref="Update"/>.
    /// </summary>
    /// <param name="keySource">The update to resolve the key from.</param>
    /// <returns>The resolved key.</returns>
    public string? ResolveKey(Update keySource);
}
