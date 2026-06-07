/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Text;
using Telegram.Bot.Types;

namespace Telegrator;

/// <summary>
/// Represents administrator rights for generating Telegram deep links.
/// </summary>
public enum BotAdminRight
{
#pragma warning disable CS1591
    ChangeInfo,
    PostMessages,
    EditMessages,
    DeleteMessages,
    RestrictMembers,
    InviteUsers,
    PinMessages,
    AddAdmins,
    Anonymous,
    ManageVideoChats,
    ManageTopics
#pragma warning restore
}

/// <summary>
/// Provides extension methods for generating Telegram deep links and public URLs.
/// </summary>
public static class TelegramLinkExtensions
{
    /// <summary>
    /// Returns a deep link (<c>tg://user?id=...</c>) that opens the user's profile inside Telegram.
    /// </summary>
    /// <param name="user">The Telegram user.</param>
    /// <returns>A <c>tg://user?id={id}</c> link.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="user"/> is <see langword="null"/>.</exception>
    public static string GetUserLink(this User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return $"tg://user?id={user.Id}";
    }

    /// <summary>
    /// Returns the public <c>https://t.me/username</c> link if the user has a username; otherwise <see langword="null"/>.
    /// </summary>
    /// <param name="user">The Telegram user.</param>
    /// <returns>A public t.me link or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="user"/> is <see langword="null"/>.</exception>
    public static string? GetPublicLink(this User user)
    {
        if (user is null)
            throw new ArgumentNullException(nameof(user));

        return string.IsNullOrWhiteSpace(user.Username) ? null : $"https://t.me/{user.Username}";
    }

    /// <summary>
    /// Returns a deep link alias for <see cref="GetUserLink(User)"/>.
    /// </summary>
    /// <param name="user">The Telegram user.</param>
    /// <returns>A <c>tg://user?id={id}</c> link.</returns>
    public static string GetDeepLink(this User user) => user.GetUserLink();

    /// <summary>
    /// Returns the public <c>https://t.me/username</c> link for a chat if it has a username; otherwise <see langword="null"/>.
    /// </summary>
    /// <param name="chat">The Telegram chat.</param>
    /// <returns>A public t.me link or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="chat"/> is <see langword="null"/>.</exception>
    public static string? GetPublicLink(this Chat chat)
    {
        if (chat is null)
            throw new ArgumentNullException(nameof(chat));

        return string.IsNullOrWhiteSpace(chat.Username) ? null : $"https://t.me/{chat.Username}";
    }

    /// <summary>
    /// Returns the direct link to a message inside a public chat or channel.
    /// </summary>
    /// <param name="message">The Telegram message.</param>
    /// <returns>
    /// A <c>https://t.me/username/messageId</c> link when the chat has a username; otherwise <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
    public static string? GetMessageLink(this Message message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));

        if (message.Chat is null)
            return null;

        string? chatLink = message.Chat.GetPublicLink();
        return chatLink is null ? null : $"{chatLink}/{message.MessageId}";
    }

    /// <summary>
    /// Converts a Telegram username into a public <c>https://t.me/username</c> link.
    /// </summary>
    /// <param name="username">The username with or without a leading <c>@</c>.</param>
    /// <returns>A public t.me link.</returns>
    /// <exception cref="ArgumentException"><paramref name="username"/> is null or empty.</exception>
    public static string ToTelegramPublicUrl(this string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));

        string normalized = username.Trim();
        if (normalized.StartsWith("@", StringComparison.Ordinal))
            normalized = normalized.Substring(1);

        return $"https://t.me/{normalized}";
    }

    /// <summary>
    /// Converts an invite hash into a <c>https://t.me/+hash</c> invite link.
    /// </summary>
    /// <param name="inviteHash">The invite hash with or without a leading <c>+</c>.</param>
    /// <returns>A t.me invite link.</returns>
    /// <exception cref="ArgumentException"><paramref name="inviteHash"/> is null or empty.</exception>
    public static string ToTelegramInviteUrl(this string inviteHash)
    {
        if (string.IsNullOrWhiteSpace(inviteHash))
            throw new ArgumentException("Invite hash cannot be null or empty.", nameof(inviteHash));

        string normalized = inviteHash.Trim();
        if (normalized.StartsWith("+", StringComparison.Ordinal))
            normalized = normalized.Substring(1);

        return $"https://t.me/+{normalized}";
    }

    /// <summary>
    /// Builds a Telegram share URL (<c>https://t.me/share/url</c>).
    /// </summary>
    /// <param name="url">The URL to share.</param>
    /// <param name="text">Optional text to include with the shared link.</param>
    /// <returns>A share URL.</returns>
    /// <exception cref="ArgumentException"><paramref name="url"/> is null or empty.</exception>
    public static string GetShareUrl(string url, string? text = null)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty.", nameof(url));

        StringBuilder builder = new StringBuilder("https://t.me/share/url?url=");
        builder.Append(Uri.EscapeDataString(url));

        if (!string.IsNullOrWhiteSpace(text))
        {
            builder.Append("&text=");
            builder.Append(Uri.EscapeDataString(text));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Returns a deep link that opens Telegram settings.
    /// </summary>
    /// <returns>A <c>tg://settings</c> link.</returns>
    public static string GetSettingsDeepLink() => "tg://settings";

    /// <summary>
    /// Returns a deep link that opens the sticker pack installation screen.
    /// </summary>
    /// <param name="setName">The sticker pack name.</param>
    /// <returns>A <c>tg://addstickers?set=...</c> link.</returns>
    /// <exception cref="ArgumentException"><paramref name="setName"/> is null or empty.</exception>
    public static string GetAddStickersDeepLink(string setName)
    {
        if (string.IsNullOrWhiteSpace(setName))
            throw new ArgumentException("Sticker set name cannot be null or empty.", nameof(setName));

        return $"tg://addstickers?set={Uri.EscapeDataString(setName)}";
    }

    /// <summary>
    /// Returns a deep link that opens a profile by username.
    /// </summary>
    /// <param name="username">The username with or without a leading <c>@</c>.</param>
    /// <returns>A <c>tg://resolve?domain=...</c> link.</returns>
    /// <exception cref="ArgumentException"><paramref name="username"/> is null or empty.</exception>
    public static string GetResolveDeepLink(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));

        string normalized = username.Trim();
        if (normalized.StartsWith("@", StringComparison.Ordinal))
            normalized = normalized.Substring(1);

        return $"tg://resolve?domain={Uri.EscapeDataString(normalized)}";
    }

    /// <summary>
    /// Returns a deep link that invites the user to a group or channel.
    /// </summary>
    /// <param name="inviteHash">The invite hash with or without a leading <c>+</c>.</param>
    /// <returns>A <c>tg://join?invite=...</c> link.</returns>
    /// <exception cref="ArgumentException"><paramref name="inviteHash"/> is null or empty.</exception>
    public static string GetJoinDeepLink(string inviteHash)
    {
        if (string.IsNullOrWhiteSpace(inviteHash))
            throw new ArgumentException("Invite hash cannot be null or empty.", nameof(inviteHash));

        string normalized = inviteHash.Trim();
        if (normalized.StartsWith("+", StringComparison.Ordinal))
            normalized = normalized.Substring(1);

        return $"tg://join?invite={Uri.EscapeDataString(normalized)}";
    }

    /// <summary>
    /// Returns a deep link that pre-fills a message text.
    /// </summary>
    /// <param name="text">The message text.</param>
    /// <returns>A <c>tg://msg?text=...</c> link.</returns>
    /// <exception cref="ArgumentException"><paramref name="text"/> is null or empty.</exception>
    public static string GetMessageDeepLink(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty.", nameof(text));

        return $"tg://msg?text={Uri.EscapeDataString(text)}";
    }

    /// <summary>
    /// Converts the enum value to its corresponding Telegram API string.
    /// This approach is strictly AOT-compatible and avoids runtime reflection.
    /// </summary>
    /// <param name="right">The administrator right to convert.</param>
    /// <returns>A string representation of the right as expected by Telegram API.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown enum value is provided.</exception>
    public static string ToApiString(this BotAdminRight right) => right switch
    {
        BotAdminRight.ChangeInfo => "change_info",
        BotAdminRight.PostMessages => "post_messages",
        BotAdminRight.EditMessages => "edit_messages",
        BotAdminRight.DeleteMessages => "delete_messages",
        BotAdminRight.RestrictMembers => "restrict_members",
        BotAdminRight.InviteUsers => "invite_users",
        BotAdminRight.PinMessages => "pin_messages",
        BotAdminRight.AddAdmins => "add_admins",
        BotAdminRight.Anonymous => "anonymous",
        BotAdminRight.ManageVideoChats => "manage_video_chats",
        BotAdminRight.ManageTopics => "manage_topics",
        _ => throw new ArgumentOutOfRangeException(nameof(right), right, null)
    };

    /// <summary>
    /// Generates a deep link to add the bot to a group.
    /// </summary>
    /// <param name="bot">The bot User object (typically obtained via GetMeAsync).</param>
    /// <param name="payload">Optional payload for the /start command (will be automatically URL-escaped).</param>
    /// <param name="adminRights">List of requested administrator rights.</param>
    /// <returns>A formatted deep link string.</returns>
    public static string GenerateAddToGroupLink(
        this User bot,
        string? payload = null,
        params BotAdminRight[] adminRights)
    {
        return GenerateDeepLink(bot, "startgroup", payload, adminRights);
    }

    /// <summary>
    /// Generates a deep link to add the bot to a channel.
    /// </summary>
    /// <param name="bot">The bot User object (typically obtained via GetMeAsync).</param>
    /// <param name="payload">Optional payload for the /start command (will be automatically URL-escaped).</param>
    /// <param name="adminRights">List of requested administrator rights.</param>
    /// <returns>A formatted deep link string.</returns>
    public static string GenerateAddToChannelLink(
        this User bot,
        string? payload = null,
        params BotAdminRight[] adminRights)
    {
        return GenerateDeepLink(bot, "startchannel", payload, adminRights);
    }

    /// <summary>
    /// Generates a deep link to redirect the user to a private chat with the bot.
    /// </summary>
    /// <param name="bot">The bot User object (typically obtained via GetMeAsync).</param>
    /// <param name="payload">Payload for the /start command (will be automatically URL-escaped).</param>
    /// <returns>A formatted deep link string.</returns>
    /// <exception cref="ArgumentException">Thrown when the bot's username is missing.</exception>
    public static string GenerateStartLink(this User bot, string payload)
    {
        if (bot == null || string.IsNullOrWhiteSpace(bot.Username))
            throw new ArgumentException("Bot User object must contain a valid Username.");

        if (string.IsNullOrWhiteSpace(payload))
            return $"https://t.me/{bot.Username}";

        return $"https://t.me/{bot.Username}?start={Uri.EscapeDataString(payload)}";
    }

    private static string GenerateDeepLink(
        User bot,
        string startParameter,
        string? payload,
        BotAdminRight[]? adminRights)
    {
        if (bot == null || string.IsNullOrWhiteSpace(bot.Username))
            throw new ArgumentException("Bot User object must contain a valid Username.");

        StringBuilder sb = new StringBuilder($"https://t.me/{bot.Username}");
        bool hasQuery = false;

        if (!string.IsNullOrWhiteSpace(payload))
        {
            sb.Append('?')
              .Append(startParameter)
              .Append('=')
              .Append(Uri.EscapeDataString(payload!));

            hasQuery = true;
        }

        if (adminRights != null && adminRights.Length > 0)
        {
            sb.Append(hasQuery ? '&' : '?').Append("admin=");
            for (int i = 0; i < adminRights.Length; i++)
            {
                if (i > 0)
                    sb.Append('+');

                sb.Append(adminRights[i].ToApiString());
            }
        }

        return sb.ToString();
    }
}
