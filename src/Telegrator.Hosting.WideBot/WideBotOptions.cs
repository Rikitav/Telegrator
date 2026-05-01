using WTelegram;

namespace Telegrator;

public class WideBotOptions
{
    public required int ApiId { get; set; }

    public required string ApiHash { get; set; }

    public string? MTProxy { get; set; }

    public bool DropPendingUpdates { get; set; }

    public SqlCommands SqlCommands { get; set; } = WTelegram.SqlCommands.Detect;
}
