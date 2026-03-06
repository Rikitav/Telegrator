using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Filters.Components;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Attribute that marks a handler to process command messages.
    /// This handler will be triggered when users send bot commands (messages starting with '/').
    /// </summary>
    public class CommandHandlerAttribute(int importance = 1) : UpdateHandlerAttribute<CommandHandler>(UpdateType.Message, importance)
    {
        /// <summary>
        /// Gets the command that was extracted from the message (without the '/' prefix and bot username).
        /// </summary>
        public string ReceivedCommand { get; private set; } = null!;

        /// <summary>
        /// Message text splited by space characters
        /// </summary>
        public string[]? Arguments { get; internal set; } = null;

        /// <summary>
        /// Checks if the update contains a valid bot command and extracts the command text.
        /// </summary>
        /// <param name="context">The filter execution context containing the update.</param>
        /// <returns>True if the update contains a valid bot command; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Update> context)
        {
            if (context.Input.Message is not { Entities.Length: > 0, Text.Length: > 0 } message)
                return false;

            MessageEntity commandEntity = message.Entities[0];
            if (commandEntity.Type != MessageEntityType.BotCommand)
                return false;

            if (commandEntity.Offset != 0)
                return false;

            ReceivedCommand = message.Text.Substring(1, commandEntity.Length - 1);
            if (ReceivedCommand.Contains('@'))
            {
                string[] split = ReceivedCommand.Split('@');
                ReceivedCommand = split[0];
            }
            
            return true;
        }
    }

    /// <summary>
    /// Abstract base class for handlers that process command messages.
    /// Provides functionality to extract and parse command arguments.
    /// </summary>
    public abstract class CommandHandler : MessageHandler
    {
        /// <summary>
        /// Cached array of command arguments.
        /// </summary>
        private string[]? _cmdArgsSplit;
        
        /// <summary>
        /// Cached string representation of command arguments.
        /// </summary>
        private string? _argsString;

        /// <summary>
        /// Gets the command that was extracted from the message.
        /// </summary>
        protected string ReceivedCommand
        {
            get => CompletedFilters.Get<CommandHandlerAttribute>(0).ReceivedCommand;
        }

        /// <summary>
        /// Gets the arguments string (everything after the command).
        /// </summary>
        protected string ArgumentsString
        {
            get => _argsString ??= ArgsStringify();
        }

        /// <summary>
        /// Gets the command arguments as an array of strings.
        /// </summary>
        protected string[] Arguments
        {
            get => _cmdArgsSplit ??= SplitArgs();
        }

        /// <summary>
        /// Splits the command arguments into an array of strings.
        /// </summary>
        /// <returns>An array of command arguments.</returns>
        private string[] SplitArgs()
        {
            if (Input.Text is not { Length: > 0 })
                return [];

            return Input.Text.Split([" "], StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
        }

        /// <summary>
        /// Extracts the arguments string from the command message.
        /// </summary>
        /// <returns>The arguments string (everything after the command).</returns>
        private string ArgsStringify()
        {
            if (Input.Text is not { Length: > 0 })
                return string.Empty;

            return Input.Text.Substring(ReceivedCommand.Length + 1);
        }
    }

    /// <summary>
    /// Abstract base class for branching handlers that process command messages.
    /// Provides functionality to extract and parse command arguments for branching scenarios.
    /// </summary>
    public abstract class BranchingCommandHandler : BranchingMessageHandler
    {
        /// <summary>
        /// Cached array of command arguments.
        /// </summary>
        private string[]? _cmdArgsSplit;
        
        /// <summary>
        /// Cached string representation of command arguments.
        /// </summary>
        private string? _argsString;

        /// <summary>
        /// Gets the command that was extracted from the message.
        /// </summary>
        protected string ReceivedCommand
        {
            get => CompletedFilters.Get<CommandHandlerAttribute>(0).ReceivedCommand;
        }

        /// <summary>
        /// Gets the arguments string (everything after the command).
        /// </summary>
        protected string ArgumentsString
        {
            get => _argsString ??= ArgsStringify();
        }

        /// <summary>
        /// Gets the command arguments as an array of strings.
        /// </summary>
        protected string[] Arguments
        {
            get => _cmdArgsSplit ??= SplitArgs();
        }

        /// <summary>
        /// Splits the command arguments into an array of strings.
        /// </summary>
        /// <returns>An array of command arguments.</returns>
        private string[] SplitArgs()
        {
            if (Input.Text is not { Length: > 0 })
                return [];

            return Input.Text.Split([" "], StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();
        }

        /// <summary>
        /// Extracts the arguments string from the command message.
        /// </summary>
        /// <returns>The arguments string (everything after the command).</returns>
        private string ArgsStringify()
        {
            if (Input.Text is not { Length: > 0 })
                return string.Empty;

            return Input.Text.Substring(ReceivedCommand.Length + 1);
        }
    }
}
