using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Base abstract class for all filter of <see cref="Message"/> updates
    /// </summary>
    public abstract class MessageFilterBase : Filter<Message>
    {
        /// <summary>
        /// Target message for filterring
        /// </summary>
        protected Message Target { get; private set; } = null!;

        /// <inheritdoc/>
        protected virtual bool CanPassBase(FilterExecutionContext<Message> context)
        {
            FromReplyChainFilter? repliedFilter = context.CompletedFilters.Get<FromReplyChainFilter>().SingleOrDefault();
            Target = repliedFilter?.Reply ?? context.Input;

            if (Target is not { Id: > 0 })
                return false;

            return true;
        }

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            if (!CanPassBase(context))
                return false;

            return CanPassNext(context);
        }

        /// <summary>
        /// Determines whether the filter can pass for the given context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract bool CanPassNext(FilterExecutionContext<Message> context);
    }

    /// <summary>
    /// Filters messages by their <see cref="MessageType"/>.
    /// </summary>
    public class MessageTypeFilter : MessageFilterBase
    {
        private readonly MessageType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTypeFilter"/> class.
        /// </summary>
        /// <param name="type">The message type to filter by.</param>
        public MessageTypeFilter(MessageType type) => this.type = type;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
            => Target.Type == type;
    }

    /// <summary>
    /// Filters messages that are automatic forwards.
    /// </summary>
    public class IsAutomaticFormwardMessageFilter : MessageFilterBase
    {
        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
            => Target.IsAutomaticForward;
    }

    /// <summary>
    /// Filters messages that are sent from offline.
    /// </summary>
    public class IsFromOfflineMessageFilter : MessageFilterBase
    {
        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
            => Target.IsFromOffline;
    }

    /// <summary>
    /// Filters service messages (e.g., chat events).
    /// </summary>
    public class IsServiceMessageMessageFilter : MessageFilterBase
    {
        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
            => Target.IsServiceMessage;
    }

    /// <summary>
    /// Filters messages that are topic messages.
    /// </summary>
    public class IsTopicMessageMessageFilter : MessageFilterBase
    {
        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
            => Target.IsTopicMessage;
    }

    /// <summary>
    /// Filters messages by dice throw value and optionally by dice type.
    /// </summary>
    public class DiceThrowedFilter : MessageFilterBase
    {
        private readonly DiceType Dice;
        private readonly int Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiceThrowedFilter"/> class for a specific value.
        /// </summary>
        /// <param name="value">The dice value to filter by.</param>
        public DiceThrowedFilter(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiceThrowedFilter"/> class for a specific dice type and value.
        /// </summary>
        /// <param name="diceType">The dice type to filter by.</param>
        /// <param name="value">The dice value to filter by.</param>
        public DiceThrowedFilter(DiceType diceType, int value) : this(value) => Dice = diceType;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            if (Target.Dice == null)
                return false;

            if (Target.Dice.Emoji != GetEmojyForDiceType(Dice))
                return false;

            return Target.Dice.Value == Value;
        }

        private static string? GetEmojyForDiceType(DiceType? diceType) => diceType switch
        {
            DiceType.Dice => "🎲",
            DiceType.Darts => "🎯",
            DiceType.Bowling => "🎳",
            DiceType.Basketball => "🏀",
            DiceType.Football => "⚽",
            DiceType.Casino => "🎰",
            _ => null
        };
    }

    /// <summary>
    /// Filters messages by matching their text with a regular expression.
    /// </summary>
    public class MessageRegexFilter : RegexFilterBase<Message>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRegexFilter"/> class with a pattern and options.
        /// </summary>
        /// <param name="pattern">The regex pattern.</param>
        /// <param name="regexOptions">The regex options.</param>
        public MessageRegexFilter(string pattern, RegexOptions regexOptions = default)
            : base(msg => msg.Text, pattern, regexOptions) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRegexFilter"/> class with a regex object.
        /// </summary>
        /// <param name="regex">The regex object.</param>
        public MessageRegexFilter(Regex regex)
            : base(msg => msg.Text, regex) { }
    }

    /// <summary>
    /// Filters messages that contain a specific entity type, content, offset, or length.
    /// </summary>
    public class MessageHasEntityFilter : MessageFilterBase
    {
        private readonly StringComparison _stringComparison = StringComparison.CurrentCulture;
        private readonly MessageEntityType? EntityType;
        private readonly string? Content;
        private readonly int? Offset;
        private readonly int? Length;

        /// <summary>
        /// Gets the entities found in the message that match the filter.
        /// </summary>
        public MessageEntity[] FoundEntities { get; set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHasEntityFilter"/> class for a specific entity type.
        /// </summary>
        /// <param name="type">The entity type to filter by.</param>
        public MessageHasEntityFilter(MessageEntityType type)
        {
            EntityType = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHasEntityFilter"/> class for a specific entity type, offset, and length.
        /// </summary>
        /// <param name="type">The entity type to filter by.</param>
        /// <param name="offset">The offset to filter by.</param>
        /// <param name="length">The length to filter by.</param>
        public MessageHasEntityFilter(MessageEntityType type, int? offset, int? length)
        {
            EntityType = type;
            Offset = offset;
            Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHasEntityFilter"/> class for a specific entity type and content.
        /// </summary>
        /// <param name="type">The entity type to filter by.</param>
        /// <param name="content">The content to filter by.</param>
        /// <param name="stringComparison">The string comparison to use.</param>
        public MessageHasEntityFilter(MessageEntityType type, string? content, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            EntityType = type;
            Content = content;
            _stringComparison = stringComparison;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHasEntityFilter"/> class for a specific entity type, offset, length, and content.
        /// </summary>
        /// <param name="type">The entity type to filter by.</param>
        /// <param name="offset">The offset to filter by.</param>
        /// <param name="length">The length to filter by.</param>
        /// <param name="content">The content to filter by.</param>
        /// <param name="stringComparison">The string comparison to use.</param>
        public MessageHasEntityFilter(MessageEntityType type, int? offset, int? length, string? content, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            EntityType = type;
            Offset = offset;
            Length = length;
            Content = content;
            _stringComparison = stringComparison;
        }

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            if (context.Input is not { Entities.Length: > 0 })
                return false;

            FoundEntities = Target.Entities.Where(entity => FilterEntity(Target.Text, entity)).ToArray();
            return FoundEntities.Length != 0;
        }

        private bool FilterEntity(string? text, MessageEntity entity)
        {
            if (EntityType != null && entity.Type != EntityType)
                return false;

            if (Offset != null && entity.Offset != Offset)
                return false;

            if (Length != null && entity.Length != Length)
                return false;

            if (Content != null)
            {
                if (text is not { Length: > 0 })
                    return false;

                if (!text.Substring(entity.Offset, entity.Length).Equals(Content, _stringComparison))
                    return false;
            }

            return true;
        }
    }
}
