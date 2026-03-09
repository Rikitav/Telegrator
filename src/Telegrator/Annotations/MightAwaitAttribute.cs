using Telegram.Bot.Types.Enums;

namespace Telegrator.Annotations;

/// <summary>
/// Attribute that says if this handler can await some of await types, that is not listed by its handler base.
/// Used for automatic collecting allowed to receiving <see cref="UpdateType"/>'s.
/// If you don't use it, you won't be able to await the updates inside handler.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class MightAwaitAttribute : Attribute
{
    private readonly UpdateType[] _updateTypes;

    /// <summary>
    /// Update types that may be awaited
    /// </summary>
    public UpdateType[] UpdateTypes => _updateTypes;

    /// <summary>
    /// main ctor of <see cref="MightAwaitAttribute"/>
    /// </summary>
    /// <param name="updateTypes"></param>
    public MightAwaitAttribute(params UpdateType[] updateTypes)
        => _updateTypes = updateTypes;
}
