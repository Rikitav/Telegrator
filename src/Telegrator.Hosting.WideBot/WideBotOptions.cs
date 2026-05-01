using Telegram.Bot;
using WTelegram;

namespace Telegrator;

/// <summary>
/// Represents configuration options for initializing and customizing the behavior of a WideBot instance.
/// </summary>
/// <remarks>Use this class to specify required API credentials, optional proxy settings, update handling
/// preferences, and SQL command detection for WideBot. All required properties must be set before using the options
/// with a WideBot instance.
/// </remarks>
public class WideBotOptions
{
    /// <inheritdoc cref="WTelegramBotClientOptions.ApiId"/>
    public required int ApiId { get; set; }

    /// <inheritdoc cref="WTelegramBotClientOptions.ApiHash"/>
    public required string ApiHash { get; set; }

    /// <inheritdoc cref="WTelegramBotClientOptions.MTProxy"/>
    public string? MTProxy { get; set; }

    /// <inheritdoc cref="WTelegramBotClientOptions.SqlCommands"/>
    public SqlCommands SqlCommands { get; set; } = WTelegram.SqlCommands.Detect;

    /// <summary>
    /// Gets or sets a value indicating whether pending updates should be discarded.
    /// </summary>
    public bool DropPendingUpdates { get; set; }
}
