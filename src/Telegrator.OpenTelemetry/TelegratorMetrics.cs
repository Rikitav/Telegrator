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

using System.Diagnostics.Metrics;

namespace Telegrator;

/// <summary>
/// Provides <see cref="Meter"/> instruments for Telegrator runtime metrics.
/// </summary>
public static class TelegratorMetrics
{
    private const string MeterName = "Telegrator";

    /// <summary>
    /// The primary <see cref="Meter"/> for Telegrator metrics.
    /// </summary>
    public static Meter Instance { get; } = new Meter(MeterName);

    /// <summary>
    /// Counter of incoming updates received by the bot.
    /// </summary>
    public static Counter<long> UpdatesReceived { get; } = Instance.CreateCounter<long>("telegrator.updates.received", description: "Number of incoming updates received");

    /// <summary>
    /// Counter of handler executions.
    /// </summary>
    public static Counter<long> HandlersExecuted { get; } = Instance.CreateCounter<long>("telegrator.handlers.executed", description: "Number of handler executions");

    /// <summary>
    /// Counter of handler execution failures.
    /// </summary>
    public static Counter<long> HandlerErrors { get; } = Instance.CreateCounter<long>("telegrator.handlers.errors", description: "Number of handler execution failures");

    /// <summary>
    /// Histogram of handler execution duration in milliseconds.
    /// </summary>
    public static Histogram<double> HandlerDuration { get; } = Instance.CreateHistogram<double>("telegrator.handlers.duration", unit: "ms", description: "Handler execution duration");

    /// <summary>
    /// UpDownCounter of currently active awaiting handlers.
    /// </summary>
    public static UpDownCounter<long> AwaitingActive { get; } = Instance.CreateUpDownCounter<long>("telegrator.awaiting.active", description: "Number of active awaiting handlers");

    /// <summary>
    /// Counter of state storage operations.
    /// </summary>
    public static Counter<long> StateOperations { get; } = Instance.CreateCounter<long>("telegrator.state.operations", description: "Number of state storage operations");

    internal static void RecordHandlerExecuted(bool success, TimeSpan duration)
    {
        HandlersExecuted.Add(1, new KeyValuePair<string, object?>("telegrator.handler.success", success));
        HandlerDuration.Record(duration.TotalMilliseconds, new KeyValuePair<string, object?>("telegrator.handler.success", success));

        if (!success)
            HandlerErrors.Add(1);
    }
}
