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
}
