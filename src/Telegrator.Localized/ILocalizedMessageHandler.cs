using Telegram.Bot.Types;

namespace Telegrator;

/// <summary>
/// Indicates that message handler utilizes localization
/// </summary>
public interface ILocalizedMessageHandler : ILocalizedHandler<Message>
{
}
