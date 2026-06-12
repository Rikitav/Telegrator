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

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers.Building;

/// <summary>
/// Internal handler used for awaiting specific update types.
/// Provides synchronization mechanism for waiting for updates of a particular type.
/// </summary>
/// <param name="handlingUpdateType">The type of update this awaiter handler waits for.</param>
internal class AwaiterHandler(UpdateType handlingUpdateType) : UpdateHandlerBase(handlingUpdateType), IHandlerContainerFactory, IDisposable
{
    /// <summary>
    /// Manual reset event used for synchronization.
    /// </summary>
    private readonly TaskCompletionSource<Update> ResetEvent = new TaskCompletionSource<Update>();

    /// <summary>
    /// Gets the update that triggered this awaiter handler.
    /// </summary>
    public Update HandlingUpdate { get; private set; } = null!;

    /// <summary>
    /// Waits for the specified update type to be received.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the wait operation.</param>
    public async Task<Update> Await(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await ResetEvent.Task.ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a handler container for this awaiter handler.
    /// </summary>
    /// <param name="describedHandler">The handler information containing the update.</param>
    /// <returns>An empty handler container.</returns>
    public IHandlerContainer CreateContainer(DescribedHandlerDescriptor describedHandler)
    {
        HandlingUpdate = describedHandler.HandlingUpdate;
        return new EmptyHandlerContainer();
    }

    /// <summary>
    /// Executes the awaiter handler by setting the reset event.
    /// </summary>
    /// <param name="container">The handler container (unused).</param>
    /// <param name="cancellation">The cancellation token (unused).</param>
    /// <returns>A completed task.</returns>
    protected override Task<Result> ExecuteInternal(IHandlerContainer container, CancellationToken cancellation)
    {
        try
        {
            if (!ResetEvent.TrySetResult(HandlingUpdate))
                ResetEvent.TrySetCanceled(cancellation);

            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex)
        {
            ResetEvent.TrySetException(ex);
            return Task.FromResult(Result.Fault());
        }
    }

    /// <inheritdoc/>
    protected override bool Dispose(bool disposing)
    {
        if (!disposing)
            return true;

        ResetEvent.TrySetCanceled();
        return true;
    }
}
