using Telegram.Bot.Types;

namespace Telegrator.Localized;

/// <summary>
/// Indicates that message handler utilizes localization
/// </summary>
public interface ILocalizedMessageHandler : ILocalizedHandler<Message>
{
}
