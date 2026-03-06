using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.Handlers;

namespace Telegrator.Filters
{
    /// <summary>
    /// Filter that checks if a command matches any of the specified aliases.
    /// Requires a <see cref="CommandHandlerAttribute"/> to be applied first to extract the command.
    /// </summary>
    /// <param name="alliases">The command aliases to check against.</param>
    public class CommandAlliasFilter(params string[] alliases) : Filter<Message>
    {
        /// <summary>
        /// Gets the command that was received and extracted by the <see cref="CommandHandlerAttribute"/>.
        /// </summary>
        public string ReceivedCommand { get; private set; } = string.Empty;

        /// <summary>
        /// Checks if the received command matches any of the specified aliases.
        /// This filter requires a <see cref="CommandHandlerAttribute"/> to be applied first
        /// to extract the command from the message.
        /// </summary>
        /// <param name="context">The filter execution context containing the completed filters.</param>
        /// <returns>True if the command matches any of the specified aliases; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            ReceivedCommand = context.CompletedFilters.Get<CommandHandlerAttribute>(0).ReceivedCommand;
            return alliases.Contains(ReceivedCommand, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
