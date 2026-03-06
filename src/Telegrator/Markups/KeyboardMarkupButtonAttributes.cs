using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegrator.Markups
{
    /// <inheritdoc cref="InlineKeyboardButton.WithCallbackData(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CallbackButtonAttribute(string name, string data) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Data to be sent in a <see cref="CallbackQuery">callback query</see> to the bot when the button is pressed, 1-64 bytes
        /// </summary>
        public string Data { get; } = data;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithCallbackGame(string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class GameButtonAttribute(string name) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithCopyText(string, CopyTextButton)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CopyTextButtonAttribute(string name, CopyTextButton copyText) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Description of the button that copies the specified text to the clipboard.
        /// </summary>
        public CopyTextButton CopyText { get; } = copyText;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithPay(string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class PayRequestButtonAttribute(string name) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithLoginUrl(string, LoginUrl)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class LoginButtonAttribute(string name, LoginUrl url) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// An HTTPS URL used to automatically authorize the user. Can be used as a replacement for the <a href="https://core.telegram.org/widgets/login">Telegram Login Widget</a>.
        /// </summary>
        public LoginUrl Url { get; } = url;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithUrl(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class UrlRedirectButtonAttribute(string name, string url) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// HTTP or tg:// URL to be opened when the button is pressed. Links <c>tg://user?id=&lt;UserId&gt;</c> can be used to mention a user by their identifier without using a username, if this is allowed by their privacy settings.
        /// </summary>
        public string Url { get; } = url;
    }
     
    /// <inheritdoc cref="InlineKeyboardButton.WithWebApp(string, WebAppInfo)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class WebAppButtonAttribute(string name, WebAppInfo webApp) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Description of the <a href="https://core.telegram.org/bots/webapps">Web App</a> that will be launched when the user presses the button. The Web App will be able to send an arbitrary message on behalf of the user using the method <see cref="TelegramBotClientExtensions.AnswerWebAppQuery">AnswerWebAppQuery</see>. Available only in private chats between a user and the bot. Not supported for messages sent on behalf of a Telegram Business account.
        /// </summary>
        public WebAppInfo AppInfo { get; } = webApp;
    }
     
    /// <inheritdoc cref="InlineKeyboardButton.WithSwitchInlineQuery(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class SwitchQueryButtonAttribute(string name, string switchInlineQuery = "") : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// If set, pressing the button will prompt the user to select one of their chats, open that chat and insert the bot's username and the specified inline query in the input field. May be empty, in which case just the bot's username will be inserted. Not supported for messages sent in channel direct messages chats and on behalf of a Telegram Business account.
        /// </summary>
        public string Query { get; } = switchInlineQuery;
    }

    /// <inheritdoc cref="InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(string, string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class QueryCurrentButtonAttribute(string name, string switchInlineQueryCurrentChat = "") : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// If set, pressing the button will insert the bot's username and the specified inline query in the current chat's input field. May be empty, in which case only the bot's username will be inserted.<br/><br/>This offers a quick way for the user to open your bot in inline mode in the same chat - good for selecting something from multiple options. Not supported in channels and for messages sent in channel direct messages chats and on behalf of a Telegram Business account.
        /// </summary>
        public string Query { get; } = switchInlineQueryCurrentChat;
    }

    /// <inheritdoc cref="KeyboardButton.WithRequestChat(string, int, bool)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestChatButtonAttribute(string name, bool chatIsChannel = true) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Signed 32-bit identifier of the request that will be received back in the <see cref="UsersShared"/> object. Must be unique within the message
        /// </summary>
        public int RequestId { get; } = new Random().Next();

        /// <summary>
        /// Pass <see langword="true"/> to request a channel chat, pass <see langword="false"/> to request a group or a supergroup chat.
        /// </summary>
        public bool ChatIsChannel { get; } = chatIsChannel;
    }

    /// <inheritdoc cref="KeyboardButton.WithRequestContact(string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestContactButtonAttribute(string name) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;
    }

    /// <inheritdoc cref="KeyboardButton.WithRequestLocation(string)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestLocationButtonAttribute(string name) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;
    }

    /// <inheritdoc cref="KeyboardButton.WithRequestPoll(string, KeyboardButtonPollType)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestPoolButtonAttribute(string name, KeyboardButtonPollType requestPoll) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// If specified, the user will be asked to create a poll and send it to the bot when the button is pressed. Available in private chats only.
        /// </summary>
        public KeyboardButtonPollType PollType { get; } = requestPoll;
    }

    /// <inheritdoc cref="KeyboardButton.WithRequestUsers(string, int, int?)"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequestUsersButtonAttribute(string name, int? maxQuantity = null) : Attribute
    {
        /// <summary>
        /// Name of button
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Signed 32-bit identifier of the request that will be received back in the <see cref="UsersShared"/> object. Must be unique within the message
        /// </summary>
        public int RequestId { get; } = new Random().Next();

        /// <summary>
        /// <em>Optional</em>. The maximum number of users to be selected; 1-10. Defaults to 1.
        /// </summary>
        public int? MaxQuantity { get; } = maxQuantity;
    }
}
