/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Telegrator.Aspects;
using Telegrator.Core;
using Telegrator.Core.Handlers;
using Telegrator.Handlers.Diagnostics;

namespace Telegrator;

/// <summary>
/// Represents handler results, allowing to communicate with router and control aspect execution
/// </summary>
public readonly record struct Result
{
    private static readonly Result ok = new(true, false, null);
    private static readonly Result fault = new(false, false, null);
    private static readonly Result next = new(true, true, null);

    /// <summary>
    /// Tell router to stop describing
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Tell router to continue describing
    /// </summary>
    public bool RouteNext { get; init; }

    /// <summary>
    /// Exact type that router should search
    /// </summary>
    public Type? NextType { get; init; }

    internal Result(bool success, bool routeNext, Type? nextType)
    {
        Success = success;
        RouteNext = routeNext;
        NextType = nextType;
    }

    /// <summary>
    /// Represents 'success'
    /// <list type="bullet">
    /// <item>Inside <see cref="IPreProcessor"/> - let handler's main block be executed</item>
    /// <item>Inside <see cref="UpdateHandlerBase.ExecuteInternal(IHandlerContainer, CancellationToken)"/> - tells <see cref="IUpdateRouter"/> that he can stop describing, as needed handler was found</item>
    /// <item>Inside <see cref="UpdateHandlerBase.FiltersFallback(FiltersFallbackReport, Telegram.Bot.ITelegramBotClient, CancellationToken)"/> - let <see cref="IUpdateRouter"/> continue describing</item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public static Result Ok()
        => ok;

    /// <summary>
    /// Represents 'fault' or 'error'. Use cases:
    /// <list type="bullet">
    /// <item>Inside <see cref="IPreProcessor"/> - interupts execution of handler, main block and <see cref="IPostProcessor"/> wont be executed</item>
    /// <item>Inside <see cref="UpdateHandlerBase.FiltersFallback(FiltersFallbackReport, Telegram.Bot.ITelegramBotClient, CancellationToken)"/> - interupts <see cref="IUpdateRouter"/>'s describing sequence</item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public static Result Fault()
        => fault;

    /// <summary>
    /// Represents 'continue'. Use cases:
    /// <list type="bullet">
    /// <item>Inside <see cref="UpdateHandlerBase.FiltersFallback(FiltersFallbackReport, Telegram.Bot.ITelegramBotClient, CancellationToken)"/> - let <see cref="IUpdateRouter"/> continue describing</item>
    /// <item>Inside <see cref="UpdateHandlerBase.ExecuteInternal(IHandlerContainer, CancellationToken)"/> - Tells <see cref="IUpdateRouter"/> to continue describing handlers</item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public static Result Next()
        => next;

    /// <summary>
    /// Represents 'chain'. Use cases:
    /// <list type="bullet">
    /// <item>Inside <see cref="UpdateHandlerBase.ExecuteInternal(IHandlerContainer, CancellationToken)"/> - Tells <see cref="IUpdateRouter"/> to continue describing handlers and execute only handlers of exact type</item>
    /// </list>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Result Next<T>()
        => new Result(true, true, typeof(T));
}
